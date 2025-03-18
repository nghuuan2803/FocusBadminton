namespace Shared.Auth
{
    public class LoginRequest
    {
        public string LoginType { get; set; }  // "google", "password", "facebook"
        public string Credential { get; set; } // AuthCode, Email|Password, IDToken
    }
}
