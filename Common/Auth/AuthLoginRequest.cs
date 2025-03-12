namespace Shared.Auth
{
    public class AuthLoginRequest
    {
        public string LoginType { get; set; }  // "google", "password", "facebook"
        public string Credential { get; set; } // AuthCode, Email|Password, IDToken
    }
}
