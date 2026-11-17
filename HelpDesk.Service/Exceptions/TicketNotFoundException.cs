namespace HelpDesk.Service.Exceptions;

public class TicketNotFoundException : HelpDeskException
{
    public TicketNotFoundException() : base("NOTFOUND", "Ticket not found.")
    { }
}
