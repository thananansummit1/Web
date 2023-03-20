using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using Web.ViewModel.Model;
using static Web.Util.AuthorizationAttribute;

namespace Web.Util
{
    public static class JWT
    {
        private static string secret = "2WV6ZDK&-m5c2f#?";
        public static string Version;
        public static string BuildVersion;


        public static string GenerateToken(UserWebReport user, string ipaddress)
        {
            byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            var _timeout = Iconfig.Configuration.GetSection("JwtTimeout:minutes").Value;
            int _addtimeout = 15;
            if (!string.IsNullOrEmpty(_timeout))
            {
                _addtimeout = int.Parse(_timeout);
            }

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Sid, user.user_empcode.ToString()),
                    new Claim("user_name", user.user_name),
                    new Claim(ClaimTypes.Version, Version )
                }),
                Expires = DateTime.UtcNow.AddMinutes(_addtimeout),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha384Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);
        }

        public static UserWebReport ValidateToken(HttpContext context, AuthorizationGroupType authorizationGroupType)
        {
            if (!context.Request.Headers.Keys.Contains("Authorization"))
            {
                throw new SecurityTokenException("Need Authorization");
            }

            string token = context.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", "");
            return DecryptionToken(token, authorizationGroupType);
        }

        private static UserWebReport DecryptionToken(string token, AuthorizationGroupType authorizationGroupType)
        {
            UserWebReport res = new UserWebReport();

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    throw new SecurityTokenException("Invalid Authorization");
                }

                byte[] key = Encoding.ASCII.GetBytes(JWT.secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                if (principal == null)
                {
                    throw new SecurityTokenException("Invalid ClaimsPrincipal");
                }

                var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(principal.FindFirst("exp").Value));
                var sdsd = exp.LocalDateTime;
                if (DateTime.Now >= sdsd)
                {
                    throw new SecurityTokenException("Expire");
                }

                Claim id = principal.FindFirst(ClaimTypes.Sid);

                if (id == null)
                {
                    throw new SecurityTokenException("Invalid ClaimType");
                }
                else
                {


                }

                return res;
            }
            catch (Exception e)
            {
                throw new SecurityTokenException(e.ToString());
            }
        }
       

        public static class Iconfig
        {
            private static IConfiguration config;
            public static IConfiguration Configuration
            {
                get
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                    config = builder.Build();
                    return config;
                }
            }
        }
    }
}
