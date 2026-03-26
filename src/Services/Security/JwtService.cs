namespace GerenciadorFuncionarios.Services.Security;

using System.Text;
using GerenciadorFuncionarios.Data;
using GerenciadorFuncionarios.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class JwtService
{
    private readonly IConfiguration _config;


    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public (string accessToken, long expiresAt) GenerateAccessToken(Funcionario usuario)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var issuer = _config["Jwt:Issuer"] ?? string.Empty;
        var audience = _config["Jwt:Audience"] ?? string.Empty;

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(30);

        var tokenOptions = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new []
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role.ToString()),
                new Claim("token-type", "access")
            },
            expires: expires,
            signingCredentials: signingCredentials
        );

       var token =  new JwtSecurityTokenHandler().WriteToken(tokenOptions);
       var epoch = new DateTimeOffset(expires).ToUnixTimeSeconds();

       return (token, epoch);
    }

    public string GenerateRefreshToken(Funcionario usuario)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var issuer = _config["Jwt:Issuer"] ?? string.Empty;
        var audience = _config["Jwt:Audience"] ?? string.Empty;

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new []
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("token-type", "refresh")
            },
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: signingCredentials
        );

       var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

       return token;
    }
}