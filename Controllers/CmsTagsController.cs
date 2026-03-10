using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers.Admin;

[ApiController]
[Route("cms/tags")]
public class CmsTagsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CmsTagsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
    {
        return await _context.Tags.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(string name)
    {
        if (await _context.Tags.AnyAsync(t => t.Name == name))
            return BadRequest("Tag already exists");

        var tag = new Tag { Name = name };
        _context.Tags.Add(tag);
        await _context.Set<Tag>().AddAsync(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
