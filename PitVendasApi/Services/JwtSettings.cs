namespace PitVendasApi.Services
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = "PitVendasApi";
        public string Audience { get; set; } = "PitVendasApp";
        public int ExpiresMinutes { get; set; } = 480;
    }
}
