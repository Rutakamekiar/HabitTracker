namespace HabitTracker.Domain.Models;

public class Record
{
    public required string Id { get; set; }

    public required string ChatId { get; set; }
    
    public required string CategoryId { get; set; }
    
    public required string CategoryIcon { get; set; }

    public required string UserId { get; set; } 

    public DateTime CreationDate { get; set; }

    public required string Username { get; set; } 
    
    public int? MessageId { get; set; }
}