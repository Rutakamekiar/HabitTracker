using HabitTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure.Repositories.TelegramUsers;

public class TelegramUserRepository: Repository<TelegramUserEntity>, ITelegramUserRepository
{
    public TelegramUserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<TelegramUserEntity> CreateOrUpdateAsync(TelegramUserEntity entity)
    {
        var existingUser = await Context.TelegramUsers.FirstOrDefaultAsync(x => x.Id == entity.Id);

        if (existingUser == null)
        {
            existingUser = (await Context.TelegramUsers.AddAsync(entity)).Entity;
        }
        else
        {
            existingUser.Username = entity.Username;
        }

        return existingUser;
    }
}