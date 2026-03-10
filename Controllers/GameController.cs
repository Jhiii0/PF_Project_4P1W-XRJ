using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers;

[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly AppDbContext _context;

    public GameController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<SubmitResponse>> SubmitGuess([FromBody] SubmitRequest request)
    {
        var puzzle = await _context.Puzzles.FindAsync(request.PuzzleId);
        if (puzzle == null) return NotFound("Puzzle not found.");

        bool isCorrect = NormalizeString(request.Guess) == NormalizeString(puzzle.Answer);
        int scoreDelta = 0;

        if (isCorrect)
        {
            scoreDelta = puzzle.Difficulty switch
            {
                Difficulty.Easy => 10,
                Difficulty.Medium => 20,
                Difficulty.Hard => 30,
                _ => 10
            };

            var progress = new UserProgress
            {
                UserId = request.UserId,
                PuzzleId = request.PuzzleId,
                SolvedAt = DateTime.UtcNow,
                ScoreDelta = scoreDelta
            };
            _context.UserProgress.Add(progress);
            await _context.SaveChangesAsync();
        }

        return new SubmitResponse
        {
            Correct = isCorrect,
            ScoreDelta = scoreDelta,
            NextAvailable = true // Simple flag for now
        };
    }

    private string NormalizeString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        // Normalize: trim, lowercase, remove spaces/hyphens
        return input.Trim().ToLowerInvariant().Replace(" ", "").Replace("-", "");
    }
}

public class SubmitRequest
{
    public string UserId { get; set; } = string.Empty;
    public int PuzzleId { get; set; }
    public string Guess { get; set; } = string.Empty;
}

public class SubmitResponse
{
    public bool Correct { get; set; }
    public int ScoreDelta { get; set; }
    public bool NextAvailable { get; set; }
}
