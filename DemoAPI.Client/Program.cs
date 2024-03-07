// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

//using(var client = new HttpClient()) {
//    var result = await client.GetAsync("http://localhost:5068/weatherforecast/");
//    Console.WriteLine($"StatusCode: {result.StatusCode} {result.ReasonPhrase}");
//    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
//}

HttpClientHandler clientHandler;
AuthToken auth;

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestHeaders = {
        { "Authorization", "Basic YnV0S19iYmdxVm95WkcxY01nak5NWEpDSkN3YTptRnZHZkdwdEp6dG1mSlAzOWZNTzFmNGZEc29h" },
    }
}
) {
    var content = new StringContent("grant_type=client_credentials") { Headers = { ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded") } };
    var result = await client.PostAsync("https://localhost:9443/oauth2/token", content);
    Console.WriteLine($"StatusCode: {result.StatusCode}");
    var body = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"Content: {body}");
    auth = JsonSerializer.Deserialize<AuthToken>(body);
    Console.WriteLine($"\nAuthorization: {auth.TokenType} {auth.AccessToken}");
}

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestHeaders = {
        { "Authorization", "Basic SFdzMGs3Q2JKcEpoTWF0UXU0YW9UODhQNGtFYTpDMXhMTmZRc0dNRUxaekJQQVRrbTk5RlR6c2th" },
    }
}
) {
    string username = "Consumidor", password = "curso";
    var content = new StringContent($"grant_type=password&username={username}&password={password}") { Headers = { ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded") } };
    var result = await client.PostAsync("https://localhost:9443/oauth2/token", content);
    Console.WriteLine($"\nStatusCode: {result.StatusCode}");
    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
    var body = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"Content: {body}");
    Console.WriteLine($"\nAuthorization: {auth.TokenType} {auth.AccessToken}");
    Console.WriteLine($"\nRefreshToken: {auth.AccessToken}");
}

clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
using(var client = new HttpClient(clientHandler) {
    DefaultRequestHeaders = {
        { "Authorization", $"{auth.TokenType} {auth.AccessToken}" }
    }
}
) {
    var result = await client.GetAsync("https://localhost:8243/pizzashack/1.0.0/menu");
    Console.WriteLine($"\nStatusCode: {result.StatusCode}");
    Console.WriteLine($"Content: {await result.Content.ReadAsStringAsync()}");
}

//Console.ReadLine();

//record Auth(
//    [JsonProperty("access_token")] string accessToken,
//    string scope,
//    [JsonProperty("token_type")] string tokenType,
//    [JsonProperty("expires_in")] long expiresIn
//    );
class AuthToken {
    [JsonPropertyName("access_token")] public string AccessToken { get; init; }
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; init; }
    [JsonPropertyName("scope")] public string Scope { get; init; }
    [JsonPropertyName("token_type")] public string TokenType { get; init; }
    [JsonPropertyName("expires_in")] public long ExpiresIn { get; init; }
}
