namespace ResourceApi.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Image> Images { get; set; } = new();
}
