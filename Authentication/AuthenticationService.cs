using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Models;
using Authentication.Interfaces;

namespace Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private const int saltSize = 32;
        public dynamic GenerateToken(User user, string secretKey)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (secretKey == null) throw new ArgumentNullException(nameof(secretKey));
            IList<Claim> Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddHours(2)).ToUnixTimeSeconds().ToString())
            };
            
            JwtSecurityToken token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        SecurityAlgorithms.HmacSha256)),
                new JwtPayload(Claims)
            );
            var toReturn = new 
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email
            };
            
            return toReturn; 
        }

        public HashSalt ComputeHashSalt(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            byte[] salt = GenerateSalt();
            byte[] hash = ComputeHashFromSalt(salt, Encoding.UTF8.GetBytes(user.Password));
            return new HashSalt(hash, salt);
        }
        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomNumber = new byte[saltSize];

                rng.GetBytes(randomNumber);

                return randomNumber;

            }
        }

        
        public byte[] ComputeHashFromSalt(byte[] Salt, byte[] PasswordBytes)
        {
            if (Salt == null) throw new ArgumentNullException(nameof(Salt));
            if (PasswordBytes == null) throw new ArgumentNullException(nameof(PasswordBytes));
            using (var hmac = new HMACSHA256(Salt))
            {
                return hmac.ComputeHash(PasswordBytes);
            }
        }
    }
}
