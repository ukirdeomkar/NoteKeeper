using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace NoteKeeper.Controllers.Api
{

    //private readonly IMapper _mapper;

    // Get /api/customer
    //public UserController(IMapper mapper)

    [Route("notekeeper/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private MyDBContext _context;
        private IConfiguration _configuration;

       // private readonly IMapper _mapper;
        public UserController(MyDBContext my_context , IConfiguration configuration)
        {
            //_context = new MyDBContext();
            //_mapper = mapper;
            _context = my_context;
            _configuration = configuration;
        }


        private User AuthenticateUser(User user)
        {
            User _user = null;
            if(user.Email=="admin@note.com" && user.Password == "admin")
            {
                _user = new User { Name = "Admin Ban Gaya"};
            }
            return _user;
        }
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt : Issuer"] , _configuration["Jwt : Audience"],
                null,expires: DateTime.Now.AddMinutes(3),
                signingCredentials:credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Login(User user)
        {
            
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(user);
            var userInDb = _context.Users.SingleOrDefault(u=>u.Email == user.Email);
            if (userInDb == null)
            {
                return BadRequest("No Account Exist");
            }
            bool verify_password = BC.Verify(user.Password, userInDb.Password);
            if(!verify_password)
            {
                return BadRequest("Wrong Password");
            }

            if (user_ != null  && verify_password)
            {
                var token = GenerateToken(user_);
                response = Ok(new { token = token });
            }
            Console.WriteLine("Login Method Executed");
            return response;
        }

        
        [HttpGet]
        public IEnumerable<User> GetUser()
        {
            var user = _context.Users.ToList();
            //var userDto = user.Select(user => _mapper.Map<UserDto>(user));
            return user;
        }

        
        [HttpGet("{id}")]
        public User GetUserById(int id)
        {
            var user = _context.Users.ToList().Single(u => u.Id == id);


            if (user == null) {
                Console.WriteLine("Not Found");
            }
            return user;

        }


        // TODO : Add Authorize for this method 
        // In the Header use Authorization key and Bearer Value + "token" generated value during login method
        //[Authorize]
        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            var users = _context.Users.ToList();
            var email = users.SingleOrDefault(u => u.Email == user.Email);
            if (email!=null) {
                return BadRequest("Email is Already Registered. Try with different email");
            }
            //Todo : AddPassword Hashing

            string passwordHash = BC.HashPassword(user.Password);

            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           // return user;
           return Ok("User Created Succesfully : " + user.Id + ": "+user.Name + ": " +user.Email+": " );
        }

        [HttpPut("{id}")]
        public void UpdateUserPassword(int id , User user)
        {
            // TODO : Use Hashing for Password and findout how to update Hashed Password ;
            // TODO : ADD extra field in user if required that can be updated
            var userinDB = _context.Users.Single(u => u.Id == id);
            if (user == null)
            {
                Console.WriteLine("Not Found User");
            }
            else
            {
                userinDB.Password = user.Password;
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
