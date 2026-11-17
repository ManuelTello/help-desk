using System.Text.Json.Serialization;

namespace HelpDesk.WebApi.Responses.Ticket;

public record FetchHistoryTicketResponse(
    [property:JsonPropertyName("history")]HistoryItemResponse[] History);

public record HistoryItemResponse(
    [property:JsonPropertyName("date_created")]DateTime DateCreated, 
    [property:JsonPropertyName("state")]string State);