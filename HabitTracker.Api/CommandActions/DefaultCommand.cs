using HabitTracker.Api.Constants;
using HabitTracker.Api.Extensions;
using HabitTracker.Api.Resources;
using HabitTracker.Application.Services.Records;
using HabitTracker.Application.Services.TelegramUsers;
using HabitTracker.Domain.Models;
using HabitTracker.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.CommandActions;

public class DefaultCommand: ICommand
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ITelegramUserService _telegramUserService;
    private readonly IRecordService _recordService;
    private readonly ILogger<DefaultCommand> _logger;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly IUnitOfWork _unitOfWork;

    public DefaultCommand(ITelegramBotClient telegramBotClient,
                          ITelegramUserService telegramUserService,
                          IRecordService recordService,
                          ILogger<DefaultCommand> logger,
                          IStringLocalizer<Messages> localizer,
                          IUnitOfWork unitOfWork)
    {
        _telegramBotClient = telegramBotClient;
        _telegramUserService = telegramUserService;
        _recordService = recordService;
        _logger = logger;
        _localizer = localizer;
        _unitOfWork = unitOfWork;
    }

    public bool MatchesCommand(string message)
    {
        return message == Commands.Default;
    }

    public async Task ExecuteAsync(HookMessage message)
    {
        var record = await _recordService.GetByMessageIdAsync(message.Chat!.Id.ToString(), message.Id);
        if (record != null)
        {
            _logger.LogInformation("Duplication found");
            // ignore duplication
            return;
        }
        
        var user = await _telegramUserService.CreateOrUpdateAsync(message.From!);
            
        var category = (await _unitOfWork.Categories.GetAsync(x => x.ChatId == message.Chat!.Id.ToString() && x.Icon == message.GetText())).FirstOrDefault();
        if (category != null)
        {
            try
            {
                await _recordService.CreateAsync(category.ToModel(), user, message);
                await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                    text: _localizer["Counted"],
                    replyToMessageId: message.Id);
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot store record: {e.Message}");
                await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                                                              text: _localizer["CantStoreRecord"],
                                                              replyToMessageId: message.Id);
            }
            
            try
            {
                var results = await _recordService.GetStatisticsByChatIdAsync(message.Chat!.Id.ToString());
                await UpdatePinnedStatistics(results, message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Cannot update statistics: {e.Message}");
                await _telegramBotClient.SendTextMessageAsync(chatId: message.Chat!.Id,
                                                              text: _localizer["CantUpdateStatistic"],
                                                              replyToMessageId: message.Id);
            }
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
            if (message.Chat != null)
            {
                await _telegramBotClient.ForceUpdatePinnedMessage(message.Chat.Id, messageText);
            }
        }
    }
}