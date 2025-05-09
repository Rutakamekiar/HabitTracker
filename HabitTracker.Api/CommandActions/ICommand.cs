using HabitTracker.Domain.Models;

namespace HabitTracker.Api.CommandActions;

public interface ICommand
{
    public bool MatchesCommand(string message);
    Task ExecuteAsync(HookMessage message);
}