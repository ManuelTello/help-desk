namespace HelpDesk.Service.Exceptions;

public class TicketAlreadyTakenException : HelpDeskException
{
    public TicketAlreadyTakenException(string name)
        : base("CONFLICT", $"The ticket is already taken by {name}")
    { }
}
