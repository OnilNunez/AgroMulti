using AgroMulti.Ui.Models;
using AgroMulti.Ui.Session;
using System.Net.Http.Json;

namespace AgroMulti.Ui.Services;

public class AuthService
{
    public async Task<bool> Login(string usuario, string password)
    {
        var request = new
        {
            usuario,
            password
        };

        var response = await ApiClient.Client.PostAsJsonAsync(
            "api/Auth/login",
            request);

        if (!response.IsSuccessStatusCode)
            return false;

        var respuesta = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (respuesta == null || !respuesta.Success || respuesta.Data == null)
            return false;

        UserSession.Token = respuesta.Data.Token;
        UserSession.Usuario = respuesta.Data.Usuario;
        UserSession.Rol = respuesta.Data.Rol;

        ApiClient.SetToken(UserSession.Token);

        return true;
    }
}