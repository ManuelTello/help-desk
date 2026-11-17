using System.Text.Json.Serialization;

namespace HelpDesk.WebApi.Responses.User;

public record ValidateCredentialsUserResponse(
    [property: JsonPropertyName("token")] string Token);
