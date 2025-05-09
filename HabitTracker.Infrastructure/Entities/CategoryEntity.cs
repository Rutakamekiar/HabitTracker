using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitTracker.Domain.Models;

namespace HabitTracker.Infrastructure.Entities;

public class CategoryEntity
{
    [Key]
    public required string Id { get; set; }

    public required string ChatId { get; set; }
    
    public required string Icon { get; set; }

    public Category ToModel()
    {
        return new Category
        {
            Id = Id,
            Icon = Icon
        };
    }
}