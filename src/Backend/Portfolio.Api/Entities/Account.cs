namespace Portfolio.Api.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
    }
}