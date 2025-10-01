using HabitTracker.Api.CommandActions;
using HabitTracker.Api.Resources;
using HabitTracker.Api.TelegramWrappers;
using HabitTracker.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace HabitTracker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BotController : ControllerBase
{
    public const string ChatId = "-686408418";

    private readonly CommandProcessor _commandProcessor;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<BotController> _logger;
    private readonly IStringLocalizer<Messages> _localizer;

    public BotController(CommandProcessor commandProcessor, HabitTrackerTelegramClientWrapper habitTrackerTelegramClientWrapper, ILogger<BotController> logger, IStringLocalizer<Messages> localizer)
    {
        _commandProcessor = commandProcessor;
        _telegramBotClient = habitTrackerTelegramClientWrapper.TelegramBotClient;
        _logger = logger;
        _localizer = localizer;
    }

    [HttpPost("webHook")]
    public async Task<IActionResult> HookUpdate(HookUpdate update)
    {
        if (string.IsNullOrEmpty(update.Message.GetText()) || update.Message.From == null || update.Message.Chat == null)
        {
            return Ok();
        }
        
        try
        {
            await _commandProcessor.ProcessCommandAsync(update.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            try
            {
                await _telegramBotClient.SendTextMessageAsync(chatId: update.Message.Chat!.Id,
                    text: _localizer["UnhandledException"],
                    replyToMessageId: update.Message.Id);
            }
            catch (Exception e2)
            {
                _logger.LogCritical(e2.Message);

                await _telegramBotClient.SendTextMessageAsync(chatId: update.Message.Chat!.Id,
                    text: _localizer["UnableToRespondTo"]);
                await _telegramBotClient.SendTextMessageAsync(chatId:update.Message.Chat!.Id,
                    text: e2.Message);
            }
        }

        return Ok();
    }
    
    [HttpGet("SendMessage")]
    public async Task<IActionResult> GetHistory(string message)
    {
        await _telegramBotClient.SendTextMessageAsync(ChatId,
            text: message);
        return Ok();
    }

    
    [HttpGet]
    [HttpHead]
    public IActionResult Get()
    {
        return Ok();
    }
}