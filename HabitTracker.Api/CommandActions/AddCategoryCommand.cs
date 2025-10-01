using System.Text.RegularExpressions;
using HabitTracker.Api.Constants;
using HabitTracker.Api.Resources;
using HabitTracker.Api.TelegramWrappers;
using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.Entities;
using HabitTracker.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace HabitTracker.Api.CommandActions;

public class AddCategoryCommand: ICommand
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryCommand(HabitTrackerTelegramClientWrapper habitTrackerTelegramClientWrapper, IStringLocalizer<Messages> localizer, IUnitOfWork unitOfWork)
    {
        _telegramBotClient = habitTrackerTelegramClientWrapper.TelegramBotClient;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public bool MatchesCommand(string message)
    {
        return Regex.IsMatch(message, Commands.AddCategory);
    }

    public async Task ExecuteAsync(HookMessage message)
    {
        var icon = message.GetText().Split(" ")[1];
        var categories = (await _unitOfWork.Categories.GetAsync(x => x.ChatId == message.Chat!.Id.ToString() && x.Icon == icon)).ToArray();
        if (categories.Length != 0)
        {
            await _telegramBotClient.SendTextMessageAsync(chatId:message.Chat!.Id, text: _localizer["CategoryAlreadyExists"], replyToMessageId: message.Id);
            return;
        }

        await _unitOfWork.Categories.AddAsync(new CategoryEntity
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = message.Chat!.Id.ToString(),
            Icon = icon
        });

        await _unitOfWork.CompleteAsync();
        
        categories = (await _unitOfWork.Categories.GetAsync(x => x.ChatId == message.Chat!.Id.ToString())).ToArray();
        var commands = new []{Commands.Start, Commands.Help, Commands.Rules}.Select(x => new KeyboardButton(x));
        var categoriesIcons = categories.Select(x => new KeyboardButton(x.Icon));
        var replyKeyboardMarkup = new ReplyKeyboardMarkup([categoriesIcons, commands])
        {
            ResizeKeyboard = true,
        };
        await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                                                      text: _localizer["CategoryWasAdded", icon],
                                                      replyMarkup: replyKeyboardMarkup,
                                                      replyToMessageId: message.Id);
    }
}