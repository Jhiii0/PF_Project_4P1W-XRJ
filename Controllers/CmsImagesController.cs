using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers.Admin;

[ApiController]
[Route("cms/images")]
public class CmsImagesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CmsImagesController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Image>>> GetImages()
    {
        return await _context.Images.Include(i => i.Tags).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Image>> CreateImage([FromBody] CreateImageRequest request)
    {
        var image = new Image { Url = request.Url };
        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetImages), new { id = image.Id }, image);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<Image>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
        
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType)) return BadRequest("Invalid image type.");
        
        if (file.Length > 5 * 1024 * 1024) return BadRequest("File too large (max 5MB).");

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(_environment.WebRootPath, "uploads", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = $"/uploads/{fileName}";
        var image = new Image { Url = imageUrl };
        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetImages), new { id = image.Id }, image);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null) return NotFound();

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/tags")]
    public async Task<IActionResult> AddTagToImage(int id, [FromBody] int tagId)
    {
        var image = await _context.Images.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
        var tag = await _context.Tags.FindAsync(tagId);

        if (image == null || tag == null) return NotFound();
        if (image.Tags.Any(t => t.Id == tagId)) return BadRequest("Tag already assigned");

        image.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}/tags/{tagId}")]
    public async Task<IActionResult> RemoveTagFromImage(int id, int tagId)
    {
        var image = await _context.Images.Include(i => i.Tags).FirstOrDefaultAsync(i => i.Id == id);
        if (image == null) return NotFound();

        var tag = image.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag == null) return NotFound();

        image.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

public class CreateImageRequest
{
    public string Url { get; set; } = string.Empty;
}
