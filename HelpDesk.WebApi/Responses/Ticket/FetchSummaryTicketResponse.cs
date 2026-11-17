using System.Text.Json.Serialization;

namespace HelpDesk.WebApi.Responses.Ticket;

public record FetchSummaryTicketResponse(
    [property:JsonPropertyName("page")] int Page, 
    [property:JsonPropertyName("page_size")]int PageSize, 
    [property:JsonPropertyName("total_tickets")]int TotalTickets, 
    [property:JsonPropertyName("summary")]SummaryItem[] Summary);

public record SummaryItem(
    [property:JsonPropertyName("id")]Guid Id, 
    [property:JsonPropertyName("created_by")]string CreatedBy ,
    [property:JsonPropertyName("subject")]string Subject, 
    [property:JsonPropertyName("priority")]string Priority, 
    [property:JsonPropertyName("category")]string Category, 
    [property:JsonPropertyName("state")]string State, 
    [property:JsonPropertyName("owner")]string? Owner,
    [property:JsonPropertyName("date_created")]DateTime DateCreated);
    