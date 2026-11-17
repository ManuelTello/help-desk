using System.Security.Claims;
using Carter;
using HelpDesk.Service.Abstractions;

namespace HelpDesk.WebApi.Modules.Ticket;

public class CloseTicketModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/ticket/{ticketId:guid}/close", async (Guid ticketId, ITicketService service, HttpContext context) =>
        {
            Guid currentUserId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await service.CloseTicket(ticketId, currentUserId);
            return Results.Ok();
        }).RequireAuthorization(policy => policy.RequireRole("Actor"))
        .WithTags("Ticket");
    }
}