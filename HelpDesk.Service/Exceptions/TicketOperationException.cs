namespace HelpDesk.Service.Exceptions;

public class TicketOperationException : HelpDeskException
{
    public TicketOperationException() : base("CONFLICT", "The state of the ticket is invalid for this operation.")
    { }
}
