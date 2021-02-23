namespace RSPGame.Services.Authentication
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(string userName, string password);
    }
}