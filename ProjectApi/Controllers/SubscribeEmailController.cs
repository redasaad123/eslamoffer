using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectApi.DTO;

namespace ProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeEmailController : ControllerBase
    {
        private readonly IUnitOfWork<SubscribeEmail> emailunitOfWork;

        public SubscribeEmailController(IUnitOfWork<SubscribeEmail> EmailunitOfWork)
        {
            emailunitOfWork = EmailunitOfWork;
        }



        [HttpGet("GetAllEmails")]

        public async Task<IActionResult> GetAllEmails()
        {
            var emails = await emailunitOfWork.Entity.GetAllAsync();

            if (emails == null || !emails.Any())
            {
                return NotFound("No emails found.");
            }

            var EmailSort = emails.OrderByDescending(e => e.ConfirmedAt).ToList();

            return Ok(EmailSort);
        }

        [HttpPost("AddSubscribeEmail")]
        public async Task<IActionResult> AddSubscribeEmail([FromBody] SubscribeEmailDTO DTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (string.IsNullOrEmpty(DTO.Email) || !DTO.Email.Contains("@") || !DTO.Email.Contains("."))
            {
                return BadRequest("Invalid email format.");
            }

            var existingEmail = emailunitOfWork.Entity.Find(e => e.Email == DTO.Email);
            if (existingEmail != null)
            {
                existingEmail.ConfirmedAt = DateTime.Now;
                await emailunitOfWork.Entity.UpdateAsync(existingEmail);
                emailunitOfWork.Save();
                return Ok(existingEmail);


            }
            var subscribeEmail = new SubscribeEmail
            {
                Id = Guid.NewGuid().ToString(),
                Email = DTO.Email,
                ConfirmedAt = DateTime.Now

            };

            await emailunitOfWork.Entity.AddAsync(subscribeEmail);
            emailunitOfWork.Save();
            return Ok(subscribeEmail);
        }

    }
}
