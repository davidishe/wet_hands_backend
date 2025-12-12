namespace WetHands.Auth.AuthService
{
    public interface IAuthService
    {
        Task<string> Login(string email, string mailFrom, string langCode);
        Task<bool> Verify(string token, string email);

    }
}