namespace HabitTracker.Domain.Models;

public class StatisticResult
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required string CategoryIcon { get; set; }
    public int Count { get; set; }
}