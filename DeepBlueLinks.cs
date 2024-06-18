using System.Net.Http.Json;

namespace DeepBlueLinks.Maui;

public class DeepBlueLinks
{
    private static DeepBlueLinks _instance;

    public static DeepBlueLinks Instance = _instance ??= new DeepBlueLinks();

    private string _apiKey;

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async void HandleUniversalLink(string url, Action<Exception, Link> OnFinished)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("Api Key Must Be Set");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
            var resp = await client.GetAsync(url + "/GetLink");
            if (resp.IsSuccessStatusCode)
            {
                var link = await resp.Content.ReadFromJsonAsync<Link>();
                if (link != null)
                {
                    OnFinished.Invoke(null, link);
                }
            }
            else
            {
                OnFinished?.Invoke(new Exception($"Error retrieving deep link: {resp.StatusCode}"), null);
            }
        }
        catch (Exception e)
        {
            OnFinished?.Invoke(new Exception($"Error retrieving deep link: {e.Message}"), null);
        }
    }
    
}