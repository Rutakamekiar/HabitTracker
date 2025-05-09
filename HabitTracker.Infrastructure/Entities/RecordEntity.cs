using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitTracker.Domain.Models;

namespace HabitTracker.Infrastructure.Entities;

public class RecordEntity
{
    [Key]
    public required string Id { get; set; }

    public required string ChatId { get; set; }
    
    public required string CategoryId { get; set; }
    [Obsolete]
    public required string CategoryIcon { get; set; }

    public DateTime CreationDate { get; set; }

    public int? MessageId { get; set; }
    
    public required string TelegramUserId { get; set; }

    [ForeignKey(nameof(TelegramUserId))]
    public TelegramUserEntity TelegramUser { get; set; } = null!;    
    
    [ForeignKey(nameof(CategoryId))]
    public CategoryEntity Category { get; set; } = null!;

    public Record ToModel()
    {
        return new Record
        {
            Id = Id,
            ChatId = ChatId,
            CategoryId = CategoryId,
            UserId = TelegramUserId,
            CreationDate = CreationDate,
            Username = TelegramUser.Username,
            CategoryIcon = Category.Icon,
            MessageId = MessageId
        };
    }
}