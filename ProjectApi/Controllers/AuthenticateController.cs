﻿using Core.AuthenticationDTO;
using Core.Interfaces;
using Core.Models;
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

        private readonly IAuthentication authentication;
        private readonly JWTSettings jwtSettings;
        

        public AuthenticateController(UserManager<AppUser> userManager,PasswordHasher<AppUser> passwordHasher,IOptions<JWTSettings> options, RoleManager<IdentityRole> roleManager
            , IUnitOfWork<AppUser> appUserUnitOfWork,  IAuthentication authentication )
        {
            this.passwordHasher = passwordHasher;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.appUserUnitOfWork = appUserUnitOfWork;
            this.authentication = authentication;
            jwtSettings = options.Value;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register( RegisterDTO dto)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await authentication.RegisterAsync(dto);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);
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

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return NotFound("Email Is NotFound");


            return Ok(user.Id);
        }


        [HttpPost("ChangePassword/{id}")]
        //[Authorize(Roles = "User")]

        public async Task<IActionResult> ChangePassword([FromForm] BasePasswordDTO dto , string id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            string idUser;

            if (id is null)
                idUser = userManager.GetUserId(HttpContext.User);
            else idUser = id.ToString();

            var user = await userManager.FindByIdAsync(idUser);
            if (user == null) 
                return NotFound("User Is NotFound");

            var hashPassword = passwordHasher.HashPassword(user, dto.NewPassword + "Abcd123#");
            user.PasswordHash = hashPassword;
            await userManager.UpdateAsync(user);
            appUserUnitOfWork.Save();
            return Ok("The Password Is Changed");


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
