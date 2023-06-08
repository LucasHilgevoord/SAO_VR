using System;

public class ObjectUnavailableException : Exception
{
    public ObjectUnavailableException() { }

    public ObjectUnavailableException(string message) : base(message) { }

    public ObjectUnavailableException(string message, Exception inner) : base(message, inner) { }
}