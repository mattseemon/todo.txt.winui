namespace Seemon.Todo.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
