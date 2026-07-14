namespace Auth.Api.Data
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public bool IsRevoked { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAtUtc;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
