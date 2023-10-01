namespace Seemon.Todo.Exceptions;

public class TaskException : Exception
{
    public TaskException() : base() { }

    public TaskException(string message) : base(message) { }

    public TaskException(string message, Exception innerException) : base(message, innerException) { }
}
