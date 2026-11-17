using System.Security.Claims;
using Carter;
using HelpDesk.Service.Abstractions;

namespace HelpDesk.WebApi.Modules.Ticket;

public class OpenTicketModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/ticket/{ticketId:guid}/open", async (Guid ticketId, ITicketService service, HttpContext context) =>
        {
            Guid currentUserId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await service.OpenTicket(ticketId, currentUserId);
            return Results.Ok();
        }).RequireAuthorization(policy => policy.RequireRole("Actor"))
        .WithTags("Ticket");
    }
}
