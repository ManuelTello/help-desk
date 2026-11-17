using System.Security.Claims;
using Carter;
using HelpDesk.Service.Abstractions;

namespace HelpDesk.WebApi.Modules.Ticket;

public class TakeTicketModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/ticket/{id:guid}/take", async (Guid id, ITicketService service, HttpContext context) =>
        {
            Guid currentUser = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await service.TakeTicket(id, currentUser);
            return Results.Ok();
        }).RequireAuthorization(policy => policy.RequireRole("Agent"))
        .WithTags("Ticket");
    }
}
