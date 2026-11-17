using System.Text.Json.Serialization;

namespace HelpDesk.WebApi.Contracts.Ticket;

public class FetchHistoryTicketContract
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("order")]
    public string Order { get; init; } = string.Empty;
}