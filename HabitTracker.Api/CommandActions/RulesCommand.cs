using HabitTracker.Api.Constants;
using HabitTracker.Api.Resources;
using HabitTracker.Domain.Models;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.CommandActions;

public class RulesCommand: ICommand
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IStringLocalizer<Messages> _localizer;

    public RulesCommand(ITelegramBotClient telegramBotClient, IStringLocalizer<Messages> localizer)
    {
        _telegramBotClient = telegramBotClient;
        _localizer = localizer;
    }

    public bool MatchesCommand(string message)
    {
        return message == Commands.Rules;
    }

    public async Task ExecuteAsync(HookMessage message)
    {
        await _telegramBotClient.SendTextMessageAsync(message.Chat!.Id,
            _localizer["RulesCommand"]);
    }
}