using HelpDesk.DAL.Data;
using HelpDesk.DAL.Models;
using HelpDesk.Service.Abstractions;
using HelpDesk.Service.Dto;
using HelpDesk.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Service.Services;

public class TicketService : ITicketService
{
    private readonly HelpDeskContext _dbContext;

    public TicketService(HelpDeskContext context)
    {
        _dbContext = context;
    }

    public async Task CreateTicket(string subject, string description, TicketCategory category, TicketPriority priority, Guid actorId)
    {
        Ticket ticket = new Ticket()
        {
            ActorId = actorId,
            Subject = subject,
            Description = description,
            Category = category,
            Priority = priority,
            DateCreated = DateTime.Now,
            DateClosed = null,
        };
        ticket.HistorySummary.Add(new HistoryEntry()
        {
            DateCreated = DateTime.Now,
            State = TicketState.Open,
        });
        await _dbContext.Tickets.AddAsync(ticket);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<HistoryDto> FetchTicketHistory(Guid ticketId, string order)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .Include(t => t.Actor)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        IEnumerable<HistoryEntry> historySorted = string.Equals(order.ToLowerInvariant(),"desc") ?
            ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated) :
            ticket.HistorySummary.OrderBy(hs => hs.DateCreated);
        HistoryDto dto = new HistoryDto(historySorted.Select(hs => new HistoryItemDto(
                hs.DateCreated, 
                hs.State))
            .ToArray());
        return dto;
    }

    public async Task<TicketSummaryDto> FetchTicketsSummary(int page, int pageSize, string orderBy)
    {
        IQueryable<Ticket> tickets = _dbContext.Tickets;
        if (orderBy == "asc")
        {
            tickets.OrderBy(t => t.DateCreated);
        }
        else
        {
            tickets.OrderByDescending(t => t.DateCreated);
        }

        TicketSummaryItemDto[] queryResult = await tickets
            .Include(t => t.HistorySummary)
            .Include(t => t.OwnershipSummary).ThenInclude(os => os.Agent)
            .Include(t => t.Actor)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(t => new TicketSummaryItemDto(
                t.Id,
                t.Actor.FullName,
                t.Subject,
                t.Priority,
                t.Category,
                t.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State,
                t.OwnershipSummary.OrderByDescending(os => os.DateCreated).First().Agent.FullName,
                t.DateCreated))
            .ToArrayAsync();
        int amountOfTickets = await _dbContext.Tickets.CountAsync();
        return new TicketSummaryDto(queryResult, page, pageSize, amountOfTickets);
    }

    public async Task OpenTicket(Guid ticketId, Guid actorId)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        TicketState state = ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State;

        if (state is not TicketState.Resolved)
        {
            throw new TicketOperationException();
        }

        ticket.HistorySummary.Add(new HistoryEntry()
        {
            DateCreated = DateTime.Now,
            State = TicketState.Open,
        });
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task CancelTicket(Guid ticketId, Guid actorId)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        TicketState state = ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State;

        if (state is TicketState.Closed)
        {
            throw new TicketOperationException();
        }

        ticket.HistorySummary.Add(new HistoryEntry()
        {
            DateCreated = DateTime.Now,
            State = TicketState.Cancelled,
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task CloseTicket(Guid ticketId, Guid actorId)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        TicketState state = ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State;

        if (state is not TicketState.Resolved)
        {
            throw new TicketOperationException();
        }

        ticket.HistorySummary.Add(new HistoryEntry()
        {
            DateCreated = DateTime.Now,
            State = TicketState.Closed,
        });
        ticket.DateClosed = DateTime.Now;

        await _dbContext.SaveChangesAsync();
    }

    public async Task ResolveTicket(Guid ticketId, Guid agentId)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        TicketState state = ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State;

        if (state is not TicketState.InProgress)
        {
            throw new TicketOperationException();
        }

        ticket.HistorySummary.Add(new HistoryEntry()
        {
            DateCreated = DateTime.Now,
            State = TicketState.Resolved,
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task TakeTicket(Guid ticketId, Guid agentId)
    {
        Ticket ticket = await _dbContext.Tickets
            .Include(t => t.HistorySummary)
            .SingleOrDefaultAsync(t => t.Id == ticketId) ?? throw new TicketNotFoundException();
        TicketState state = ticket.HistorySummary.OrderByDescending(hs => hs.DateCreated).First().State;
        DateTime currentDate = DateTime.Now;
        
        if (state is TicketState.InProgress)
        {
            User user = await _dbContext.Users.SingleAsync(u => u.Id == agentId);
            throw new TicketAlreadyTakenException(user.FullName);
        }

        if (state is TicketState.Cancelled or TicketState.Resolved or TicketState.Closed )
        {
            throw new TicketOperationException();
        }

        ticket.HistorySummary.Add(new HistoryEntry()
        {
            State = TicketState.InProgress,
            DateCreated = currentDate,
        });
        ticket.OwnershipSummary.Add(new OwnershipEntry()
        {
            AgentId = agentId,
            DateCreated = currentDate,
        });
        
        await _dbContext.SaveChangesAsync();
    }
}