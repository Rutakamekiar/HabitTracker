using HabitTracker.Infrastructure.Repositories.Categories;
using HabitTracker.Infrastructure.Repositories.Records;
using HabitTracker.Infrastructure.Repositories.TelegramUsers;

namespace HabitTracker.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public ApplicationDbContext Context { get; set; }

    private IRecordRepository? _records;
    private ITelegramUserRepository? _telegramUsers;
    private ICategoryRepository? _categoryRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        Context = context;
        // commented to decrease DB usage
        // Context.Database.EnsureCreated();
    }

    public IRecordRepository Records
    {
        get
        {
            return _records ??= new RecordRepository(Context);
        }
    }

    public ITelegramUserRepository TelegramUsers
    {
        get
        {
            return _telegramUsers ??= new TelegramUserRepository(Context);
        }
    }
    
    public ICategoryRepository Categories
    {
        get
        {
            return _categoryRepository ??= new CategoryRepository(Context);
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}