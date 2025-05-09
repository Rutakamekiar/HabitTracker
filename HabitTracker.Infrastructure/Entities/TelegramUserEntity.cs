using System.ComponentModel.DataAnnotations;
using HabitTracker.Domain.Models;

namespace HabitTracker.Infrastructure.Entities;

public class TelegramUserEntity
{
    [Key]
    public required string Id { get; set; }

    public required string Username { get; set; }

    public ICollection<RecordEntity> Records { get; set; } = new List<RecordEntity>();

    public TelegramUser ToModel()
    {
        return new TelegramUser
        {
            Id = Id,
            Username = Username,
        };
    }
}