namespace ClickView.GoodStuff.AspNetCore.Authentication.TokenValidation;

using System;

internal sealed class InvalidLogoutTokenException : Exception
{
    public InvalidLogoutTokenException(string message) : base(message)
    {
    }

    public InvalidLogoutTokenException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
