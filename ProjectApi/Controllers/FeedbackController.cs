using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTO;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class FeedbackController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork<FeedBack> feedBackUnitOfWork;

        public FeedbackController( IUnitOfWork<FeedBack> feedBackUnitOfWork)
        {
            this.userManager = userManager;
            this.feedBackUnitOfWork = feedBackUnitOfWork;
        }


        [HttpGet("GetFeedBack")]
        //[Authorize("AdminRole")]
        public async Task<IActionResult> FeedBack()
        {

            var feedbacks = await feedBackUnitOfWork.Entity.GetAllAsync();
            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound("No feedback found.");
            }


            return Ok(feedbacks);
        }
        [HttpPost("AddMessage")]
        public async Task<IActionResult> AddMessage(AddMessageDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);  

            var feedback = new FeedBack
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email ,
                Name = dto.Name,
                Message = dto.Message,
                country = dto.country,
                DateTime = DateTime.UtcNow


            };

            return Ok(feedback);

        }

        [HttpDelete("DeleteMessage/{feedBackId}")]
        //[Authorize("AdminRole")]
        public async Task< IActionResult> DeleteMessage(string feedBackId)
        {
            var feedback = await feedBackUnitOfWork.Entity.GetAsync(feedBackId);
            if (feedback == null)
                return NotFound();

            feedBackUnitOfWork.Entity.Delete(feedback);
            feedBackUnitOfWork.Save();
            return Ok("The Message Is Deleted");


        }
    }
}
