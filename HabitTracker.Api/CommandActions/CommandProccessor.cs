using HabitTracker.Api.Constants;
using HabitTracker.Api.Resources;
using HabitTracker.Application.Services.Records;
using HabitTracker.Domain.Models;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.CommandActions;

public class CommandProcessor
{
    private readonly IRecordService _recordService;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IReadOnlyDictionary<string, ICommand> _commands;
    private readonly IStringLocalizer<Messages> _localizer;

    public CommandProcessor(IEnumerable<ICommand> commands, IRecordService recordService, ITelegramBotClient telegramBotClient, IStringLocalizer<Messages> localizer)
    {
        _recordService = recordService;
        _telegramBotClient = telegramBotClient;
        _localizer = localizer;
        _commands = commands.ToDictionary(x => x.Command);
    }

    public async Task ProcessCommandAsync(HookMessage message)
    {
        if (_commands.TryGetValue(message.GetText(), out var command))
        {
            await command.ExecuteAsync(message);
        }
        else if (message.GetText() == Commands.Redo)
        {
            message = message.ReplyToMessage!;
            var record = await _recordService.GetByMessageIdAsync(message.Chat!.Id.ToString(), message.Id);
            if (record == null)
            {
                await _commands[Commands.Default].ExecuteAsync(message);
            }
            else
            {
                await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                    text: _localizer["RecordAlreadyExists"],
                    replyToMessageId: message.Id);
            }
        }
        else
        {
            await _commands[Commands.Default].ExecuteAsync(message);
        }
    }
}