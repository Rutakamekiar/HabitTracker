using HabitTracker.Api.Constants;
using HabitTracker.Api.Extensions;
using HabitTracker.Api.Resources;
using HabitTracker.Application.Services.Records;
using HabitTracker.Domain.Models;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.CommandActions;

public class CancelCommand: ICommand
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IRecordService _recordService;
    private readonly IStringLocalizer<Messages> _localizer;

    public CancelCommand(ITelegramBotClient telegramBotClient, IRecordService recordService, IStringLocalizer<Messages> localizer)
    {
        _telegramBotClient = telegramBotClient;
        _recordService = recordService;
        _localizer = localizer;
    }

    public bool MatchesCommand(string message)
    {
        return message == Commands.Cancel;
    }

    public async Task ExecuteAsync(HookMessage message)
    {
        if (message.ReplyToMessage == null)
        {
            await _telegramBotClient.SendTextMessageAsync(message.Chat!.Id,
                _localizer["ReplyWasNotSelected"]);
            return;
        }
        
        var record = await _recordService.GetByMessageIdAsync(message.Chat?.Id.ToString()!, message.ReplyToMessage!.Id);
        if (record == null)
        {
            await _telegramBotClient.SendTextMessageAsync(message.Chat!.Id,
                _localizer["CantFindRecord"]);
            return;
        }
        
        if (record.UserId != message.From?.Id.ToString())
        {
            await _telegramBotClient.SendTextMessageAsync(message.Chat!.Id,
                _localizer["UnauthorizedAccess", message.From!.Username]);
            return;
        }
        
        await _recordService.DeleteAsync(record.Id);
        await _telegramBotClient.DeleteMessageAsync(message.Chat!.Id,
            message.ReplyToMessage.Id);
        await _telegramBotClient.SendTextMessageAsync(message.Chat!.Id,
            _localizer["RecordWasDeleted"],
            replyToMessageId: message.Id);
        try
        {
            var results = await _recordService.GetStatisticsByChatIdAsync(message.Chat!.Id.ToString());
            await UpdatePinnedStatistics(results, message);
        }
        catch (Exception)
        {
            await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                text: _localizer["CantUpdateStatistic"],
                replyToMessageId: message.Id);
        }
    }
    
    private async Task UpdatePinnedStatistics(IEnumerable<StatisticResult> statisticResults, HookMessage message)
    {
        var messages = statisticResults.GroupBy(x => x.CategoryIcon)
            .Select(category => category.OrderByDescending(x => x.Count).ToArray())
            .Select(orderedCategory => $"""
                                        {orderedCategory.First().CategoryIcon}
                                        {string.Join('\n', orderedCategory.Select(x => $"{x.Username}: {x.Count}"))}
                                        """)
            .ToList();

        if (messages.Any())
        {
            var messageText = string.Join("\n\n", messages);
            await _telegramBotClient.ForceUpdatePinnedMessage(message.Chat!.Id, messageText);
        }
    }
}