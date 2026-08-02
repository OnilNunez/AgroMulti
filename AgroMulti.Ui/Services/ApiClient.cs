using System.Net.Http.Headers;

namespace AgroMulti.Ui.Services;

public static class ApiClient
{
    public static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:5126/")
    };

    public static void SetToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public static void ClearToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}