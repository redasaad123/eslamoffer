
using Core.Interfaces;
using Core.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTO;
using System.Net;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    public class AccountController : ControllerBase
    {
        
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork<AppUser> unitOfWork;

        public AccountController( UserManager<AppUser> userManager , IUnitOfWork<AppUser> unitOfWork )
        {
            
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet("GetUsers")]
        //[Authorize("AdminRole")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (users == null)
                return NotFound("Not Found Any Users");

            var user = users.Select(x => new 
            {
                Id = x.Id,
                Address = x.Address,
                Email = x.Email,
                Name = x.Name,
                PhoneNamber = x.PhoneNumber,
                UrlPhoto = x.Photo,
                roles =  userManager.GetRolesAsync(x).Result,
            }).ToList();

            return Ok(user);
        }


        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("This User Not Registed");

            //long compressedSize = new FileInfo($"wwwroot/ProfilePhoto/{user.Photo}").Length;

            var mapUser = new
            {
                Id = user.Id,
                Address = user.Address,
                Email = user.Email,
                Name = user.Name,
                PhoneNamber = user.PhoneNumber,
                UrlPhoto = user.Photo,
                roles = await userManager.GetRolesAsync(user)
            };
            return Ok(mapUser);
        }

        [HttpPut("EditUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO dto)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("This User Not Registed");


            //long originalSize = dto.Photo.Length;

            user.Email = dto.Email ?? user.Email;
            user.Name = dto.Name ?? user.Name;
            user.PhoneNumber = dto.PhoneNamber ?? user.PhoneNumber;

            await userManager.UpdateAsync(user);
            var mapUser = new 
            {
                Id = user.Id,
                Address = user.Address,

                Email = user.Email,
                FirstName = user.Name.Split(" ").First(),
                LastName = user.Name.Split(" ").Last(),

                PhoneNamber = user.PhoneNumber,
                UrlPhoto = user.Photo,

            };

            return Ok(mapUser);
        }


        [HttpDelete ("DeleteUser/{Id}")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null) 
                return NotFound("This User Not Registed");
            await userManager.DeleteAsync(user);
            return Ok("User Is Deleted !");



        }
    }
}
