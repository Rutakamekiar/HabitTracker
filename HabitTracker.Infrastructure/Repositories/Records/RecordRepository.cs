using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure.Repositories.Records;

public class RecordRepository: Repository<RecordEntity>, IRecordRepository
{
    public RecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<RecordEntity?> GetByMessageIdAsync(string chatId, int messageId)
    {
        return Context.Records.Include(x => x.TelegramUser).Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.ChatId == chatId && x.MessageId == messageId);
    }

    public IReadOnlyCollection<StatisticResult> GetStatisticsByChatId(string chatId)
    {
        var records = Context.Records
            .Include(x => x.TelegramUser)
            .Include(x => x.Category)
            .Where(x => x.ChatId == chatId).ToArray();
        var result = records
            .GroupBy(x => new { x.TelegramUserId, x.TelegramUser.Username })
            .SelectMany(userGroup => userGroup
                .GroupBy(x => new { x.CategoryId, CategoryIcon = x.Category.Icon })
                .Select(categoryGroup => new StatisticResult
                {
                    UserId = userGroup.Key.TelegramUserId,
                    Username = userGroup.Key.Username,
                    CategoryIcon = categoryGroup.Key.CategoryIcon,
                    Count = categoryGroup.Count(),
                })
            )
            .ToList();
        
        return result;
    }
}