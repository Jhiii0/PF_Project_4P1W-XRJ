namespace ResourceApi.Models;

public class Puzzle
{
    public int Id { get; set; }
    public string Answer { get; set; } = string.Empty;
    public string? Hint { get; set; }
    public Difficulty Difficulty { get; set; }
    
    public List<Image> Images { get; set; } = new();
    public List<Pack> Packs { get; set; } = new();
}
