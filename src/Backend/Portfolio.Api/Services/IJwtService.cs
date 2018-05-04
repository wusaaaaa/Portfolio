namespace Portfolio.Api.Services
{
    public interface IJwtService
    {
        string Create(int accountId, string emailAddress);
        bool Validate(string token);
    }
}