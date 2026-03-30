using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EliseuBatista99.Core.Authentication
{
    public class AuthenticationHelper
    {
        public static string EncryptPassword<TUser>(TUser user, string? password) where TUser : class
        {
            var _passwordHasher = new PasswordHasher<TUser>();

            return _passwordHasher.HashPassword(user, password ?? string.Empty);
        }

        public static PasswordVerificationResult DecryptPassword<TUser>(TUser user, string? passwordHash, string? password) where TUser : class
        {
            var _passwordHasher = new PasswordHasher<TUser>();

            return _passwordHasher.VerifyHashedPassword(user, passwordHash ?? string.Empty, password ?? string.Empty);
        }

        public static string GenerateAccessToken(IEnumerable<Claim> claims, JwtSettings settings)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
