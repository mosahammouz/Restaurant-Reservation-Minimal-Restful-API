using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace RestaurantReservation.API.Authentication;

public class JwtTokenGenerator
{
    private readonly RSA _rsa;

    public JwtTokenGenerator(RSA rsa)
    {
        _rsa = rsa;
    }
    public string GenerateToken(int employeeId, string role)
    {
       
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, employeeId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

   
        var securityKey = new RsaSecurityKey(_rsa);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
       
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}