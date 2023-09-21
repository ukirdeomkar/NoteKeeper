using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Collections;

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
       // private readonly IMapper _mapper;
        public UserController(MyDBContext my_context)
        {
            //_context = new MyDBContext();
            //_mapper = mapper;
            _context = my_context;
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
