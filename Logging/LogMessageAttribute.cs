using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class LogMessageAttribute : Attribute
{
    public string Message { get; }
    public string Application { get; }
    public string Environment { get; }

    public LogMessageAttribute()
    {
        Message = "No custom message provided.";
    }

    public LogMessageAttribute(string message, string application, string environment)
    {
        Message = message;
        Application = application;
        Environment = environment;

    }
}
