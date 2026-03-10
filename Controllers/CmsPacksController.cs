using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers.Admin;

[ApiController]
[Route("cms/packs")]
public class CmsPacksController : ControllerBase
{
    private readonly AppDbContext _context;

    public CmsPacksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pack>>> GetPacks()
    {
        return await _context.Packs.Include(p => p.Puzzles).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Pack>> CreatePack([FromBody] CreatePackRequest request)
    {
        var pack = new Pack
        {
            Name = request.Name,
            Description = request.Description,
            IsPublished = false,
            DisplayOrder = request.DisplayOrder
        };

        _context.Packs.Add(pack);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPacks), new { id = pack.Id }, pack);
    }

    [HttpPost("{id}/puzzles")]
    public async Task<IActionResult> AddPuzzleToPack(int id, [FromBody] int puzzleId)
    {
        var pack = await _context.Packs.Include(p => p.Puzzles).FirstOrDefaultAsync(p => p.Id == id);
        var puzzle = await _context.Puzzles.FindAsync(puzzleId);

        if (pack == null || puzzle == null) return NotFound();
        if (pack.Puzzles.Any(p => p.Id == puzzleId)) return BadRequest("Puzzle already in pack");

        pack.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishPack(int id)
    {
        var pack = await _context.Packs.FindAsync(id);
        if (pack == null) return NotFound();

        pack.IsPublished = true;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePack(int id)
    {
        var pack = await _context.Packs.FindAsync(id);
        if (pack == null) return NotFound();

        _context.Packs.Remove(pack);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreatePackRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}
