using Telegram.Bot;

namespace HabitTracker.Api.TelegramWrappers;

public class HabitTrackerTelegramClientWrapper
{
    public readonly ITelegramBotClient TelegramBotClient;
    
    public HabitTrackerTelegramClientWrapper(ITelegramBotClient telegramBotClient)
    {
        TelegramBotClient = telegramBotClient;
    }
}