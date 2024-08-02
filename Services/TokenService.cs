using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieBookingBackend.Services
{
    public class TokenService : ITokenService
    {
        public async Task<SymmetricSecurityKey> GenerateTokenKey()
        {
            string _secretKey = await GetSecretAsync();
            Console.WriteLine(_secretKey);
            SymmetricSecurityKey _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            return _key;
        }

        public async Task<string> GetSecretAsync()
        {
            const string secretName = "AceTicketsJwtKey";
            var keyVaultName = "AceTicketsVault";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = await client.GetSecretAsync(secretName);
            Console.WriteLine(secret);
            return secret.Value.Value;
        }

        /// <summary>
        /// Generates a JWT token for a specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>A JWT token as a string.</returns>
        public async Task<string> GetUserToken(User user)
        {
            string token = string.Empty;
            var claims = new List<Claim>(){
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var _key = await GenerateTokenKey();
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var myToken = new JwtSecurityToken(null, null, claims, expires: DateTime.Now.AddDays(2), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(myToken);
            return token;
        }
    }
}
