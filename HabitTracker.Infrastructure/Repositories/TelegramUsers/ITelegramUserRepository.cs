using HabitTracker.Infrastructure.Entities;

namespace HabitTracker.Infrastructure.Repositories.TelegramUsers;

public interface ITelegramUserRepository: IRepository<TelegramUserEntity>
{
    Task<TelegramUserEntity> CreateOrUpdateAsync(TelegramUserEntity entity);
}