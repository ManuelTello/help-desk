using System.Security.Claims;
using Carter;
using HelpDesk.Service.Abstractions;

namespace HelpDesk.WebApi.Modules.Ticket;

public class ResolveTicketModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/ticket/{ticketId:guid}/resolve", async (Guid ticketId, ITicketService service, HttpContext context) =>
        {
            Guid currentUserId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await service.ResolveTicket(ticketId,currentUserId);
            return Results.Ok();
        }).RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithTags("Ticket");
    }
}
