namespace HelpDesk.Service.Exceptions;

public class UserInvalidCredentailsException : HelpDeskException
{
    public UserInvalidCredentailsException() : base("NOTFOUND", "Invalid username or password.")
    { }
}
