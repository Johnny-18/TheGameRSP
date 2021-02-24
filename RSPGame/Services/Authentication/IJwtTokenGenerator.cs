namespace RSPGame.Services.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(string userName, string password);
    }
}