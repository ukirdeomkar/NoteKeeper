using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
            if (user_ != null)
            {
                var token = GenerateToken(user_);
                response = Ok(new { token = token });
            }
            Console.WriteLine("Login Method Executed");
            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<User> GetUser()
        {
            var user = _context.Users.ToList();
            //var userDto = user.Select(user => _mapper.Map<UserDto>(user));
            return user;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public User GetUserById(int id)
        {
            var user = _context.Users.ToList().Single(u => u.Id == id);


            if (user == null) {
                Console.WriteLine("Not Found");
            }
            return user;

        }

        [AllowAnonymous]
        [HttpPost]
        public User CreateUser(User user)
        {
            var users = _context.Users.ToList();

            //Todo : Check Unique Email COndition 
            //Todo : AddPassword Hashing

            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
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
