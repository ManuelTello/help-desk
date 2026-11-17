namespace HelpDesk.Service.Exceptions;

public class UserNameAlreadyTaken : HelpDeskException
{
    public UserNameAlreadyTaken(string username) : base("CONFLICT", $"Username '{username}' already taken.")
    { }
}
