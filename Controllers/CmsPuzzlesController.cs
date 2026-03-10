using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers.Admin;

[ApiController]
[Route("cms/puzzles")]
public class CmsPuzzlesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CmsPuzzlesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Puzzle>>> GetPuzzles()
    {
        return await _context.Puzzles.Include(p => p.Images).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Puzzle>> CreatePuzzle([FromBody] CreatePuzzleRequest request)
    {
        if (request.ImageIds.Count != 4)
            return BadRequest("A puzzle must have exactly 4 images.");

        if (string.IsNullOrWhiteSpace(request.Answer))
            return BadRequest("Answer cannot be empty.");

        var images = await _context.Images.Where(i => request.ImageIds.Contains(i.Id)).ToListAsync();
        if (images.Count != 4)
            return BadRequest("One or more image IDs are invalid.");

        var puzzle = new Puzzle
        {
            Answer = request.Answer.Trim(),
            Hint = request.Hint,
            Difficulty = request.Difficulty,
            Images = images
        };

        _context.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPuzzles), new { id = puzzle.Id }, puzzle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePuzzle(int id)
    {
        var puzzle = await _context.Puzzles.FindAsync(id);
        if (puzzle == null) return NotFound();

        _context.Puzzles.Remove(puzzle);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreatePuzzleRequest
{
    public string Answer { get; set; } = string.Empty;
    public string? Hint { get; set; }
    public Difficulty Difficulty { get; set; }
    public List<int> ImageIds { get; set; } = new();
}
