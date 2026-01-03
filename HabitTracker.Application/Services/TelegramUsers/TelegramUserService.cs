using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.Entities;
using HabitTracker.Infrastructure.UnitOfWork;

namespace HabitTracker.Application.Services.TelegramUsers;

public class TelegramUserService : ITelegramUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public TelegramUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TelegramUser> CreateOrUpdateAsync(HookFrom from)
    {
        var result = await _unitOfWork.TelegramUsers.CreateOrUpdateAsync(new TelegramUserEntity
        {
            Id = from.Id.ToString(),
            Username = from.GetUsername(),
        }).ContinueWith(x => x.Result.ToModel());
        await _unitOfWork.CompleteAsync();
        return result;
    }
}