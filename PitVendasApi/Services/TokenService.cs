using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PitVendasApi.Identity;

namespace PitVendasApi.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiraEm) Gerar(ApplicationUser usuario);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

        public (string Token, DateTime ExpiraEm) Gerar(ApplicationUser usuario)
        {
            var expiraEm = DateTime.UtcNow.AddMinutes(_settings.ExpiresMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id),
                new(ClaimTypes.Name, usuario.UserName ?? string.Empty),
                new(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new("nome", usuario.Nome),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiraEm,
                signingCredentials: credenciais);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
        }
    }
}
