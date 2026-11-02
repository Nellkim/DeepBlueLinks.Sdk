using System.Net.Http.Json;

// Creates a link, updates it and deletes it again with Deep Blue Links' API.
// Needs DBL_API_KEY and DBL_APP_ID; DBL_API_URL defaults to https://thedbl.ink.
var apiUrl = Environment.GetEnvironmentVariable("DBL_API_URL") ?? "https://thedbl.ink";
var apiKey = Environment.GetEnvironmentVariable("DBL_API_KEY");
if (string.IsNullOrEmpty(apiKey) || !int.TryParse(Environment.GetEnvironmentVariable("DBL_APP_ID"), out var appId))
{
    Console.Error.WriteLine("Set DBL_API_KEY to your app's API key (the sandbox key outside production) and DBL_APP_ID to its id.");
    return 1;
}

using var http = new HttpClient { BaseAddress = new Uri($"{apiUrl.TrimEnd('/')}/") };
http.DefaultRequestHeaders.Add("Api-Key", apiKey);

var response = await http.PostAsJsonAsync("api/links", new
{
    appId,
    fullUrl = "/products/1234",
    title = "Spring sale",
    description = "Up to 50% off this week only.",
    imageUrl = "https://example.com/images/spring-sale.png",
    fallbackUrl = "https://example.com/spring-sale",
});
await EnsureSuccessAsync(response);
var link = await response.Content.ReadFromJsonAsync<LinkResponse>();
Console.WriteLine($"Created link {link!.Id}: {link.ShortUrl}");

response = await http.PutAsJsonAsync($"api/links/{link.Id}", new
{
    appId,
    fullUrl = "/products/1234",
    title = "Spring sale, extended",
    description = "",
    imageUrl = "",
    fallbackUrl = "",
});
await EnsureSuccessAsync(response);
var updated = await response.Content.ReadFromJsonAsync<LinkResponse>();
Console.WriteLine($"Updated its title to \"{updated!.Title}\"");

response = await http.DeleteAsync($"api/links/{link.Id}");
await EnsureSuccessAsync(response);
Console.WriteLine($"Deleted link {link.Id}");
return 0;

static async Task EnsureSuccessAsync(HttpResponseMessage response)
{
    if (response.IsSuccessStatusCode)
        return;
    var body = await response.Content.ReadAsStringAsync();
    throw new HttpRequestException($"Deep Blue Links answered {(int)response.StatusCode}: {(body.Length > 0 ? body : "(empty body)")}");
}

record LinkResponse(int Id, string FullUrl, string ShortUrl, string Title);
