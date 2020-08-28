using System.Security.Claims;

namespace Client.Main.Utilities
{
    public interface IJWTContainerModel
    {
        Claim[] Claims { get; set; }
        int ExpireMinutes { get; set; }
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
    }
}