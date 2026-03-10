using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceApi.Data;
using ResourceApi.Models;

namespace ResourceApi.Controllers;

[ApiController]
[Route("packs")]
public class PacksController : ControllerBase
{
    private readonly AppDbContext _context;

    public PacksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pack>>> GetPacks([FromQuery] bool random = false)
    {
        var query = _context.Packs
            .Where(p => p.IsPublished)
            .Include(p => p.Puzzles)
            .AsQueryable();

        var packs = await query.ToListAsync();

        if (random)
        {
            var rng = new Random();
            packs = packs.OrderBy(a => rng.Next()).ToList();
        }
        else
        {
            packs = packs.OrderBy(p => p.DisplayOrder).ToList();
        }

        return packs;
    }
}
