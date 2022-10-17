using CRMApp.Common;
using CRMAppEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRMApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IOptionsSnapshot<JWTSettings> jwtSettingsOpt;

        public LoginController(IOptionsSnapshot<JWTSettings> jwtSettingsOpt)
        {
            this.jwtSettingsOpt = jwtSettingsOpt;
        }

        [HttpGet("Login/{userName}/{password}")]
        public async Task<JsonTemplate<string>> Login(string userName,string password)
        {
            using (DBContext ctx = new())
            {
               var result = await (from e in ctx.UserSet
                               where e.Name == userName && e.Password== password
                                   select e).CountAsync();
                if (result>0)
                {
                    List<Claim> claims = new List<Claim>();
                   // claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));//用户ID
                    claims.Add(new Claim(ClaimTypes.Name, userName));//用户名


                    byte[] secBytes = Encoding.UTF8.GetBytes(jwtSettingsOpt.Value.SecKey);
                    var secKey = new SymmetricSecurityKey(secBytes);
                    var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256Signature);
                    var tokenDescriptor = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddSeconds(jwtSettingsOpt.Value.ExpireSecond)
                        , signingCredentials: credentials);
                    string jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
                    return new JsonTemplate<string> { StatusCode = "200", Msg = "success", Content = jwt };
                    //string json= JsonConvert.SerializeObject(new JsonTemplate { StatusCode = "200", Msg = "success", Content = jwt });
                    //return json;
                }
                else
                {
                    return new JsonTemplate<string> { StatusCode = "200", Msg = "success", Content = "用户或密码不正确！" };
                    //string json = JsonConvert.SerializeObject(new JsonTemplate { StatusCode = "301", Msg = "success", Content = "用户或密码不正确！" });
                    //return json;


                }
            }
            
        }
    }
}
