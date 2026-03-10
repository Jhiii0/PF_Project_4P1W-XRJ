namespace ResourceApi.Models;

public class Image
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public List<Tag> Tags { get; set; } = new();
    public List<Puzzle> Puzzles { get; set; } = new();
}
