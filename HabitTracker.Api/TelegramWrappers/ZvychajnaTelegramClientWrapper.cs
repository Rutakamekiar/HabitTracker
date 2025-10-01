using Telegram.Bot;

namespace HabitTracker.Api.TelegramWrappers;

public class ZvychajnaTelegramClientWrapper
{
    public readonly ITelegramBotClient TelegramBotClient;
    
    public ZvychajnaTelegramClientWrapper(ITelegramBotClient telegramBotClient)
    {
        TelegramBotClient = telegramBotClient;
    }
}