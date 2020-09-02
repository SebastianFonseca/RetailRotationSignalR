using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Client.Main.Utilities
{
    public class JWTContainerModel : IJWTContainerModel
    {
        #region Public Methods
        public int ExpireMinutes { get; set; } = 10440; // 1 day.
        public string SecretKey { get; set; } = "TW9zaGVFcmV6UHJpdmF0ZUtleQ=="; // This secret key should be moved to some configurations outter server.
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;

        public Claim[] Claims { get; set; }
        #endregion



    }
}
