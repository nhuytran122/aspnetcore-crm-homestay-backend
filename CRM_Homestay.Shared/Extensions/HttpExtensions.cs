using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRM_Homestay.Core.Extensions;

public static class HttpExtensions
{
    public static async Task<T> GetAPIAsync<T>(this HttpClient httpClient, [Required] string URL)
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage = await httpClient.GetAsync(URL);
        return await ReturnApiResponse<T>(httpResponseMessage);
    }

    private static async Task<T> ReturnApiResponse<T>(HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            string? jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync() ?? null;
            return JsonConvert.DeserializeObject<T>(jsonResponse!)!;
        }
        return default!;
    }
}