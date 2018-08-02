using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BaseCoreApi.Settings;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace BaseCoreApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticateController : Controller
    {
        
        private readonly JWTOptions jWTOptions;
        
        public AuthenticateController(IOptionsSnapshot<JWTOptions> jwtoptions)
        {
            this.jWTOptions = jwtoptions.Value;
        }

        //TODO: change this from basic auth 
        //create response to go here if not authorized 
        //Force HTTPS ? 

        [HttpPost]
        public IActionResult Post()
        {
            string authHeader = Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                var authorizationHeader = Request.Headers["Authorization"].First();
                var key = authorizationHeader.Split(' ')[1];
                var cridentials = Encoding.UTF8.GetString(
                    Convert.FromBase64String(key)).Split(':');
                var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                    (jWTOptions.ServerSecret));

                if (cridentials[0] == "john" && cridentials[1] == "john")
                {
                    var result = GenerateToken(serverSecret);
                    return Ok(result);
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        //TODO: refactor this 

        private object GenerateToken(SecurityKey serverSecret)
        {
            var now = DateTime.Now;
            var identity = new ClaimsIdentity();
            var signingCridentials = new SigningCredentials(
                serverSecret,
                SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(
                jWTOptions.Issuer,
                jWTOptions.Audience,
                identity,
                now,
                now.Add(TimeSpan.FromHours(1)),
                now,
                signingCridentials);

            var encodedJwt = handler.WriteToken(token);
            return encodedJwt; 

        }
    }
}
