using HabitTracker.Api.Constants;
using HabitTracker.Api.Resources;
using HabitTracker.Api.TelegramWrappers;
using HabitTracker.Application.Services.Records;
using HabitTracker.Domain.Models;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.CommandActions;

public class CommandProcessor
{
    private readonly IRecordService _recordService;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IEnumerable<ICommand> _commands;
    private readonly IStringLocalizer<Messages> _localizer;

    public CommandProcessor(IEnumerable<ICommand> commands, IRecordService recordService, HabitTrackerTelegramClientWrapper wrapper, IStringLocalizer<Messages> localizer)
    {
        _recordService = recordService;
        _telegramBotClient = wrapper.TelegramBotClient;
        _localizer = localizer;
        _commands = commands;
    }

    public async Task ProcessCommandAsync(HookMessage message)
    {
        var command = _commands.FirstOrDefault(x => x.MatchesCommand(message.GetText()));
        if (command != null)
        {
            await command.ExecuteAsync(message);
        }
        else
        {
            if (message.GetText() == Commands.Redo)
            {
                message = message.ReplyToMessage!;
                var record = await _recordService.GetByMessageIdAsync(message.Chat!.Id.ToString(), message.Id);
                if (record != null)
                {
                    await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                        text: _localizer["RecordAlreadyExists"],
                        replyToMessageId: message.Id);
                    return;
                }
            }

            var defaultCommand = _commands.FirstOrDefault(x => x.MatchesCommand(Commands.Default))!;
            await defaultCommand.ExecuteAsync(message);
        }
    }
}