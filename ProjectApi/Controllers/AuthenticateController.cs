using Core.AuthenticationDTO;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly PasswordHasher<AppUser> passwordHasher;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork<AppUser> appUserUnitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;
        private readonly IAuthentication authentication;
        private readonly SendEmailServices sendMessage;
        private readonly JwtSettings jwtSettings;
        

        public AuthenticateController(PasswordHasher<AppUser> passwordHasher,IOptions<JwtSettings> options, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager
            , IUnitOfWork<AppUser> appUserUnitOfWork, Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting , IAuthentication authentication, SendEmailServices sendMessage )
        {
            this.passwordHasher = passwordHasher;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.appUserUnitOfWork = appUserUnitOfWork;
            this.hosting = hosting;
            this.authentication = authentication;
            this.sendMessage = sendMessage;
            jwtSettings = options.Value;
        }


        [HttpPost("Register")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authentication.RegisterAsync(dto);

            //if(!result.IsAuthenticated)
            //    return BadRequest(result.Message);
            //if (!string.IsNullOrEmpty(result.RefreshToken))
            //    setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogInDTo dto)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authentication.LoginAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

                setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);


            return Ok(result);

        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            var result = await authentication.RefreshTokenAsync(refreshToken);

            if(!result.IsAuthenticated)
                return BadRequest(result);

            setRefreshTokenInCookie(result.RefreshToken,result.RefreshTokenExpiration);

            return Ok(result);

        }

        private void setRefreshTokenInCookie(string refreshToken, DateTime expire)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expire.ToLocalTime(),
            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

        }




    }
}
