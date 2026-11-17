namespace HelpDesk.Service.Exceptions;

public class HelpDeskException : Exception
{
    public string Code { get; init; }

    public HelpDeskException(string code, string message) : base(message)
    {
        Code = code;
    }
}
