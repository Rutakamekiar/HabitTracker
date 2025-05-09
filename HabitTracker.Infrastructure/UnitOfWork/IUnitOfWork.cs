using HabitTracker.Infrastructure.Repositories.Categories;
using HabitTracker.Infrastructure.Repositories.Records;
using HabitTracker.Infrastructure.Repositories.TelegramUsers;

namespace HabitTracker.Infrastructure.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ApplicationDbContext Context { get; }

    IRecordRepository Records { get; }
    ITelegramUserRepository TelegramUsers { get; }
    ICategoryRepository Categories { get; }

    Task<int> CompleteAsync();
}