using System;
using System.Security.Claims;

namespace ToDoList.SharedClientLibrary.Services
{
    public interface ITokenParser
    {
        ClaimsPrincipal GetClaimsPrincipal(string token);
        DateTimeOffset GetExpiryDate(string token, ClaimsPrincipal claimsPrincipal = null);
    }
}
