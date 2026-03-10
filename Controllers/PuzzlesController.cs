using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers;

[ApiController]
[Route("puzzles")]
public class PuzzlesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PuzzlesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("next")]
    public async Task<ActionResult<Puzzle>> GetNextPuzzle([FromQuery] int packId, [FromQuery] string userId)
    {
        var pack = await _context.Packs
            .Include(p => p.Puzzles)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == packId);

        if (pack == null) return NotFound("Pack not found.");

        // Get solved puzzles for this user recently (e.g., last 24 hours "cooldown")
        var cooldownThreshold = DateTime.UtcNow.AddHours(-24);
        var solvedRecently = await _context.UserProgress
            .Where(up => up.UserId == userId && up.SolvedAt > cooldownThreshold)
            .Select(up => up.PuzzleId)
            .ToListAsync();

        var availablePuzzles = pack.Puzzles
            .Where(p => !solvedRecently.Contains(p.Id))
            .ToList();

        if (!availablePuzzles.Any())
        {
            // If all solved, just give a random one from the pack (cycle back)
            availablePuzzles = pack.Puzzles;
        }

        if (!availablePuzzles.Any()) return NotFound("No puzzles in this pack.");

        var rng = new Random();
        var nextPuzzle = availablePuzzles[rng.Next(availablePuzzles.Count)];

        return nextPuzzle;
    }
}
