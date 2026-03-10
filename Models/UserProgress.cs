namespace ResourceApi.Models;

public class UserProgress
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PuzzleId { get; set; }
    public DateTime SolvedAt { get; set; }
    public int ScoreDelta { get; set; }
}
