using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public static class JwtHelper
{
    public static string GenerateJwtToken(Guid userId, Guid personId, int roleId, string secretKey, string issuer, string audience, int expiresInMinutes)
    {
        var Role = roleId switch
        {
            1 => "Admin",
            2 => "Doctor",
            3 => "Patient",
            4 => "Pharmacist",
            5 => "Staff",
            _ => "Unknown"
        };

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim("personId", personId.ToString()),
        new Claim("userId", userId.ToString()),
        new Claim("roleId", roleId.ToString()),
       new Claim(ClaimTypes.Role, Role),
     

    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
