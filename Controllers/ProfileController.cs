using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers;

[ApiController]
[Route("profile")]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("progress")]
    public async Task<ActionResult<ProfileSummary>> GetProgress([FromQuery] string userId)
    {
        var solvedCount = await _context.UserProgress
            .Where(up => up.UserId == userId)
            .CountAsync();

        var totalScore = await _context.UserProgress
            .Where(up => up.UserId == userId)
            .SumAsync(up => up.ScoreDelta);

        return new ProfileSummary
        {
            Solved = solvedCount,
            Attempts = solvedCount, // Simple placeholder: assuming all recorded solved were successes
            Score = totalScore
        };
    }
}

public class ProfileSummary
{
    public int Solved { get; set; }
    public int Attempts { get; set; }
    public int Score { get; set; }
}
