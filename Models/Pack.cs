namespace ResourceApi.Models;

public class Pack
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
    
    public List<Puzzle> Puzzles { get; set; } = new();
}
