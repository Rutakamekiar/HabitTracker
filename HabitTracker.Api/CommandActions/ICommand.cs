using HabitTracker.Domain.Models;

namespace HabitTracker.Api.CommandActions;

public interface ICommand
{
    public string Command { get; }
    Task ExecuteAsync(HookMessage message);
}