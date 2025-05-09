using HabitTracker.Infrastructure.Entities;

namespace HabitTracker.Infrastructure.Repositories.Records;

public interface IRecordRepository : IRepository<RecordEntity>
{
    Task<RecordEntity?> GetByMessageIdAsync(string chatId, int messageId);
}