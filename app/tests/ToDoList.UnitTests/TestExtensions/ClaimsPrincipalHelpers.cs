using System;
using System.Security.Claims;

namespace TestExtensions
{
    public static class ClaimsPrincipalHelpers
    {
        public static ClaimsPrincipal CreateClaimsPrincipal(DateTimeOffset expiryDate)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim("exp", expiryDate.ToUnixTimeSeconds().ToString())
                }, "jwtAuthentication"));
        }

        public static object CreateClaimsPrincipal(object expiryDate) => throw new NotImplementedException();
    }
}
