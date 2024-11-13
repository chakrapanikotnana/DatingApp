using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
{
        _config = config;
    }   
 public string GenerateToken(AppUser user)
    {
        var tokenKey= _config["TokenKey"]?? throw new Exception("Token Key cannot be accessed from appsettings");
        if(tokenKey.Length<64) throw new Exception("Token Key needs to be longer");
        var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims= new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.UserName)
        };
        var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor=new SecurityTokenDescriptor
        {
            Subject=new ClaimsIdentity(claims),
            Expires=DateTime.UtcNow.AddHours(1),
            SigningCredentials=creds
        };
        var tokenHandler=new JwtSecurityTokenHandler();
        var token=tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
