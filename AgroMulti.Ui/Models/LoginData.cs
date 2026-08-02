namespace AgroMulti.Ui.Models;

public class LoginData
{
    public string Token { get; set; } = "";

    public DateTime Expira { get; set; }

    public string Usuario { get; set; } = "";

    public string Rol { get; set; } = "";
}