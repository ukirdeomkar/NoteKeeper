using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteKeeper.Interfaces;
using NoteKeeper.Models;
using NoteKeeper.Repository;
using BC = BCrypt.Net.BCrypt;


namespace NoteKeeper.Controllers.Api
{

    [Route("notekeeper/[controller]")]
    [ApiController]
    public class UserMoqController : ControllerBase 
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserMoqController(IUserRepository userRepository, IConfiguration configuration) {
            //_context = new MyDBContext();
            //_mapper = mapper;
            _userRepository = userRepository;
            _configuration = configuration;

        }


        [HttpGet("{id}")]
        public User GetMoqUserById(int id)
        {
            var user = _userRepository.GetUserById(id);


            if (user == null)
            {
                Console.WriteLine("Not Found");
            }
            return user;

        }

        [HttpPost]
        public IActionResult CreateMoqUser(User user)
        {
            bool success = false;

            // Check if the email is already registered
            var existingUser = _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Email is Already Registered. Try with a different email" });
            }

            // TODO: Add password hashing
            string passwordHash = BC.HashPassword(user.Password);
            user.Password = passwordHash;

            try
            {
                // Create the user using the repository
                _userRepository.CreateUser(user);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok(new { success });
        }


    }
}
