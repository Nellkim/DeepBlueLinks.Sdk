using System.Net;
using System.Net.Http.Json;

namespace DeepBlueLinks.Maui;

public class DeepBlueLinks
{
    private const string _baseApiUrl = "https://thedbl.ink";

    private static DeepBlueLinks _instance;

    public static DeepBlueLinks Instance = _instance ??= new DeepBlueLinks();

    public Action<Link> OnDeferredLink;

    private string _apiKey;

    private HttpClient _httpClient;

    public void SetApiKey(string apiKey, Action<Link> onDeferredLinkFound = null)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        
        if (onDeferredLinkFound != null)
        {
            // Run the deferred link check asynchronously
            Task.Run(async () =>
            {
                var link = await CheckForDeferredDeepLinkAsync();
                if (link != null)
                {
                    onDeferredLinkFound.Invoke(link);
                }
            });
        }
    }

    /// <summary>
    /// Handles processing a shortened universal link to return the full path. If an error occurs, the Exception property of the OnFinished Action will be set.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="OnFinished"></param>
    public async void HandleUniversalLink(string url, Action<Exception, Link> OnFinished)
    {
        try
        {
            var resp = await _httpClient.GetAsync(url + "/GetLink");
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

    /// <summary>
    /// This method Creates a new Link. A valid App Id is required. This can be found on the Deep Blue Link Dashboard at https://app.deepbluelinks.io 
    /// </summary>
    /// <param name="link"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<CreateOrUpdateLinkResponse> CreateLinkAsync(CreateLinkRequest link)
    {
        if (link.AppId is null || link.AppId.Value == 0)
            throw new Exception("App Id must be valid");

        try
        {
            var resp = await _httpClient.PostAsync($"{_baseApiUrl}/Link", JsonContent.Create(link));
            if (resp.IsSuccessStatusCode)
            {
                var createdLink = await resp.Content.ReadFromJsonAsync<CreateOrUpdateLinkResponse>();
                return createdLink;
            }

            throw new Exception("There was a problem creating the link");
        }
        catch (Exception e)
        {
            throw new Exception("There was a problem creating the link");
        }
    }

    /// <summary>
    /// This method Updates an existing Link. A valid App Id is required. This can be found on the Deep Blue Link Dashboard at https://app.deepbluelinks.io 
    /// </summary>
    /// <param name="linkId"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<CreateOrUpdateLinkResponse> UpdateLinkAsync(int linkId, CreateLinkRequest link)
    {
        if (link.AppId is null || link.AppId.Value == 0)
            throw new Exception("App Id must be valid.");

        if (linkId == 0)
            throw new Exception("linkId must be valid.");

        try
        {
            var resp = await _httpClient.PutAsync($"{_baseApiUrl}/Link{linkId}", JsonContent.Create(link));
            if (resp.IsSuccessStatusCode)
            {
                var updatedLink = await resp.Content.ReadFromJsonAsync<CreateOrUpdateLinkResponse>();
                return updatedLink;
            }

            throw new Exception("There was a problem updating the link");
        }
        catch (Exception e)
        {
            throw new Exception("There was a problem updating the link");
        }
    }

    /// <summary>
    /// This method deletes a link by Id.
    /// </summary>
    /// <param name="linkId"></param>
    public async Task DeleteLinkAsync(int linkId)
    {
        if (linkId == 0)
            throw new Exception("linkId must be valid.");

        try
        {
            var resp = await _httpClient.DeleteAsync($"{_baseApiUrl}/Link{linkId}");
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception("There was a problem deleting the link");
            }
        }
        catch (Exception e)
        {
            throw new Exception("There was a problem deleting the link");
        }
    }

    private async Task<Link> CheckForDeferredDeepLinkAsync()
    {
        if (!string.IsNullOrEmpty(_apiKey))
        {
            var fingerprint = await GetDeviceFingerprintAsync();

            try
            {
                var resp = await _httpClient.PostAsync($"{_baseApiUrl}/Link/checkdeferred", JsonContent.Create(fingerprint));

                if (resp.IsSuccessStatusCode && resp.StatusCode == HttpStatusCode.OK)
                {
                    var link = await resp.Content.ReadFromJsonAsync<Link>();
                
                    if(OnDeferredLink != null)
                        OnDeferredLink.Invoke(link);
                
                    return link;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        return null;
    }

    private async Task<DeviceFingerprint> GetDeviceFingerprintAsync()
    {
    
        string screenResolution = "";
        
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var width = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
            var height = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;;
            screenResolution = $"{width}x{height}";
        });
    
        string timezone = TimeZoneInfo.Local.Id;
    
        return new DeviceFingerprint
        {
            ScreenResolution = screenResolution,
            Timezone = timezone,
            IpAddress = "0.0.0.0" // Placeholder for the IP address, to be set server-side
        };
    }
    

    internal class DeviceFingerprint
    {
        public string IpAddress { get; set; }
        public string ScreenResolution { get; set; }
        public string Timezone { get; set; }
        
        public int LinkId { get; set; }  // Include LinkId in the fingerprint
    }
    
}