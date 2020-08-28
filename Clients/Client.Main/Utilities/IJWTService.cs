using System.Collections.Generic;
using System.Security.Claims;

namespace Client.Main.Utilities
{
    public interface IJWTService
    {         
           string SecretKey { get; set; }

            bool IsTokenValid(string token);
            string GenerateToken(IJWTContainerModel model);
            IEnumerable<Claim> GetTokenClaims(string token);
        
    }
}