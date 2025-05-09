using HabitTracker.Domain.Models;

namespace HabitTracker.Application.Services.TelegramUsers;

public interface ITelegramUserService
{
    Task<TelegramUser> CreateOrUpdateAsync(HookFrom from);
}