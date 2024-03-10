using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Extention;
using SchoolSystemAPI.Data;
using SchoolSystemAPI.Models;
using SchoolSystemAPI.Repository;
namespace SchoolSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthinticationController : ControllerBase
    {

        private IUserRepository userRepository;
        public AuthinticationController(AppDbContext appDbContext,
                                        IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            var foundUser = userRepository.GetUserByUserName(user.UserName).Result;

            if (foundUser == null || !ExtintionClass.CheckPassword(user.Password, foundUser.Password)) 
            {
                return BadRequest("Invalid user name or password!");
            }

            else return Ok(foundUser);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] User user)
        {
            var FindUser = userRepository.GetUserByUserName(user.UserName).Result;
            if (FindUser != null)
            {
                return BadRequest("This User Name Already Exist");
            }
            user.Password = ExtintionClass.HashPassword(user.Password);

            return Ok(userRepository.AddUser(user).Result);

        }
    }
}