namespace AgroMulti.Ui.Models;

public class LoginResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = "";

    public LoginData? Data { get; set; }
}