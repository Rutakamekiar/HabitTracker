using HabitTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure.Repositories.Categories;

public class CategoryRepository: Repository<CategoryEntity>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}