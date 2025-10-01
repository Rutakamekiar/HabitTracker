using HabitTracker.Api.TelegramWrappers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace HabitTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly ITelegramBotClient _telegramBotClient;
    private const string ChatId = "-1002852432502";
    public NotificationsController(ZvychajnaTelegramClientWrapper zvychajnaTelegramClientWrapper)
    {
        _telegramBotClient = zvychajnaTelegramClientWrapper.TelegramBotClient;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        await _telegramBotClient.SendTextMessageAsync(ChatId, request.Message);
        return Ok();
    }
}

public class NotificationRequest
{
    public required string Message { get; set; }
}