namespace ClubStat.Infrastructure.Models
{
    public class Session
    {

        public Guid UserId { get; set; }
        public required string Token { get; set; }
        public DateTime Expiry { get; set; } = DateTime.Now.AddMinutes(10);
    }
}
