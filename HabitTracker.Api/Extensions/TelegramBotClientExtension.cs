using Telegram.Bot;

namespace HabitTracker.Api.Extensions;

public static class TelegramBotClientExtensions
{
    public static async Task ForceUpdatePinnedMessage(this ITelegramBotClient telegramBotClient, long chatId, string text)
    {
        var chat = await telegramBotClient.GetChatAsync(chatId);

        try
        {
            if (chat.PinnedMessage == null)
            {
                var messageToPin = await telegramBotClient.SendTextMessageAsync(chat.Id, text);
                await telegramBotClient.PinChatMessageAsync(chat.Id, messageToPin.MessageId);
            }
            else
            {
                await telegramBotClient.EditMessageTextAsync(chat.Id, chat.PinnedMessage.MessageId, text);
            }
        }
        catch (Exception)
        {
            var messageToPin = await telegramBotClient.SendTextMessageAsync(chat.Id, text);
            await telegramBotClient.PinChatMessageAsync(chat.Id, messageToPin.MessageId);
        }
    }
}