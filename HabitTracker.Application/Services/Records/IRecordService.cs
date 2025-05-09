using HabitTracker.Domain.Models;

namespace HabitTracker.Application.Services.Records;

public interface IRecordService
{
    Task CreateAsync(Category category, TelegramUser user, HookMessage message);
    Task<List<StatisticResult>> GetStatisticsByChatIdAsync(string chatId);
    Task<IEnumerable<Record>> GetHistory(string chatId,
        DateTime? startDate,
        DateTime? endDate,
        string? userName,
        string? categoryId);
    Task<Record?> GetByIdAsync(string id);
    Task DeleteAsync(string id);
    Task<Record?> GetLastAsync(string chatId, string userId, string categoryId);

    Task<Record?> GetByMessageIdAsync(string chatId, int messageId);
}