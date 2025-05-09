﻿using Newtonsoft.Json;

namespace HabitTracker.Domain.Models;

public class HookUpdate
{
    public HookMessage Message { get; set; } = new();
}

public class HookMessage
{
    [JsonProperty(Required = Required.Always, PropertyName = "message_id")]
    public int Id { get; set; }
    
    public HookChat? Chat { get; set; }

    public HookFrom? From { get; set; }

    [JsonProperty(Required = Required.Always, PropertyName = "date")]
    public long UnixTimestamp { get; set; }

    public string? Text { private get; set; }
    
    public HookSticker? Sticker { private get; set; }
    
    [JsonProperty(PropertyName = "reply_to_message")]
    public HookMessage? ReplyToMessage { get; set; }

    public DateTime GetDate()
    {
        var epoch = new DateTime(1970,
                                 1,
                                 1,
                                 0,
                                 0,
                                 0,
                                 DateTimeKind.Utc);
        return DateTime.SpecifyKind(epoch.AddSeconds(UnixTimestamp), DateTimeKind.Utc);
    }
    
    public static long GetUnixTimestamp(DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(date.ToUniversalTime() - epoch).TotalSeconds;
    }


    public string GetText()
    {
        if (!string.IsNullOrEmpty(Sticker?.Emoji))
        {
            return Sticker.Emoji;
        }
        
        if (!string.IsNullOrEmpty(Text))
        {
            return Text;
        }

        return string.Empty;
    }
}

public class HookChat
{
    public long Id { get; set; }
}

public class HookFrom
{
    [JsonProperty(Required = Required.Always, PropertyName = "id")]
    public long Id { get; set; }

    [JsonProperty(Required = Required.Always, PropertyName = "username")]
    public string Username { get; set; } = default!;
}

public class HookSticker
{
    [JsonProperty(PropertyName = "emoji")]
    public string Emoji { get; set; } = string.Empty;
}