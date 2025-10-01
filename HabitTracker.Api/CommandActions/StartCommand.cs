using HabitTracker.Api.Constants;
using HabitTracker.Api.Resources;
using HabitTracker.Api.TelegramWrappers;
using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabitTracker.Api.CommandActions;

public class StartCommand: ICommand
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IUnitOfWork _unitOfWork;

    public StartCommand(HabitTrackerTelegramClientWrapper wrapper, IStringLocalizer<Messages> localizer, IUnitOfWork unitOfWork)
    {
        _telegramBotClient = wrapper.TelegramBotClient;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public bool MatchesCommand(string message)
    {
        return message == Commands.Start;
    }

    public async Task ExecuteAsync(HookMessage message)
    {
        var categories = await _unitOfWork.Categories.GetAsync(x => x.ChatId == message.Chat!.Id.ToString());
        var categoriesIcons = categories.Select(x => new KeyboardButton(x.Icon));
        var commands = new []{Commands.Start, Commands.Help, Commands.Rules}.Select(x => new KeyboardButton(x));
        var replyKeyboardMarkup = new ReplyKeyboardMarkup([categoriesIcons, commands])
        {
            ResizeKeyboard = true,
        };
        
        await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                                                      text: _localizer["StartCommand"],
                                                      replyMarkup: replyKeyboardMarkup,
                                                      allowSendingWithoutReply: true);
    }
}