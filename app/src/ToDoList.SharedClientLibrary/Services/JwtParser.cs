using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace ToDoList.SharedClientLibrary.Services
{
    public class JwtTokenParser : ITokenParser
    {
        public ClaimsPrincipal GetClaimsPrincipal(string token)
        {
            return string.IsNullOrWhiteSpace(token)
                ? throw new ArgumentException($"'{nameof(token)}' cannot be null or whitespace.", nameof(token))
                : new(new ClaimsIdentity(ParseClaimsFromToken(token), "jwtAuthentication"));
        }

        public DateTimeOffset GetExpiryDate(string token, ClaimsPrincipal claimsPrincipal = null)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException($"'{nameof(token)}' cannot be null or whitespace.", nameof(token));

            var claims = ParseClaimsFromToken(token);
            claimsPrincipal ??= new(new ClaimsIdentity(claims, "jwtAuthentication"));

            string expiryDate = claimsPrincipal.FindFirst("exp").Value;

            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryDate));
        }

        private static IEnumerable<Claim> ParseClaimsFromToken(string token)
        {
            List<Claim> claims = new();

            string payload = token.Split('.')[1];
            byte[] jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
