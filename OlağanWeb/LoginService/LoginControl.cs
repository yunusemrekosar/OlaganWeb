using Newtonsoft.Json;
using OlağanWeb.LoginService;
using OlağanWeb.Models;
using System.Net.Http;
using System.Net;

namespace OlağanWeb.Methods;
public class LoginControl : ILoginControl
{
    public bool Control(string session)
    {
        if (session == "Giriş Başarılı")
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
