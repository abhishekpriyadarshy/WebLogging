using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class LogMessageAttribute : Attribute
{
    public string Message { get; }

    public LogMessageAttribute(string message)
    {
        Message = message;
    }
}
