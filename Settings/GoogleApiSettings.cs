namespace DAMApi.Settings
{
    public class GoogleApiSettings
    {
        public  required string ClientId { get; set; } 

        public required string ClientSecret { get; set; } 

        public required string RedirectUri { get; set; } 

        public  required string RefreshToken { get; set; }
    }
}
