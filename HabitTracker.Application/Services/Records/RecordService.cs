using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.Entities;
using HabitTracker.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Application.Services.Records;

public class RecordService : IRecordService
{
    private readonly IUnitOfWork _unitOfWork;

    public RecordService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(Category category, TelegramUser user, HookMessage message)
    {
        var record = new RecordEntity
        {
            Id = Guid.NewGuid()
                .ToString(),
            ChatId = message.Chat!.Id.ToString(),
            CategoryId = category.Id,
            CategoryIcon = category.Icon,
            TelegramUserId = user.Id,
            CreationDate = message.GetDate(),
            MessageId = message.Id,
        };
        
        await _unitOfWork.Records.AddAsync(record);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task<Record?> GetByMessageIdAsync(string chatId, int messageId)
    {
        var entity = await _unitOfWork.Records.GetByMessageIdAsync(chatId, messageId);
        return entity?.ToModel();
    }

    public async Task<List<StatisticResult>> GetStatisticsByChatIdAsync(string chatId)
    {
        var kyivTz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
        var kyivNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kyivTz);
        var year = kyivNow.Year;

        var kyivStart = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var kyivNext = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(kyivStart, kyivTz);
        var nextUtc = TimeZoneInfo.ConvertTimeToUtc(kyivNext, kyivTz);
        
        var records = await _unitOfWork.Records.GetAsync(x => 
            x.ChatId == chatId 
         && x.CreationDate >= startUtc 
         && x.CreationDate < nextUtc,
            [x => x.TelegramUser, x => x.Category]);
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

    public async Task<IEnumerable<Record>> GetHistory(string chatId, DateTime? startDate, DateTime? endDate, string? userId, string? categoryId)
    {
        var recordsQueryable = _unitOfWork.Records.AsQueryable(x => x.ChatId == chatId);
        if (startDate != null)
        {
            recordsQueryable = recordsQueryable.Where(x => x.CreationDate >= startDate);
        }

        if (endDate != null)
        {
            recordsQueryable = recordsQueryable.Where(x => x.CreationDate <= endDate);
        }

        if (!string.IsNullOrWhiteSpace(userId))
        {
            recordsQueryable = recordsQueryable.Where(x => x.TelegramUserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            recordsQueryable = recordsQueryable.Where(x => x.CategoryId == categoryId);
        }

        return await recordsQueryable.ToListAsync().ContinueWith(x => x.Result.Select(y => y.ToModel()));
    }

    public Task<Record?> GetByIdAsync(string id)
    {
        return _unitOfWork.Records.GetByIdAsync(id).ContinueWith(x => x.Result?.ToModel());
    }
    
    public async Task DeleteAsync(string id)
    {
        _unitOfWork.Records.Delete(id);
        await _unitOfWork.CompleteAsync();
    }
    
    public async Task<Record?> GetLastAsync(string chatId, string userId, string categoryId)
    {
        var recordsQueryable = await _unitOfWork.Records.AsQueryable(x => x.ChatId == chatId && x.TelegramUserId == userId && x.CategoryId == categoryId)
            .OrderByDescending(x => x.CreationDate)
            .FirstOrDefaultAsync();

        return recordsQueryable?.ToModel();
    }
}