using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Models;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("notekeeper/[controller]")]
    public class ShareNoteController : Controller
    {
        //private MyDBContext _context;
        //private readonly IMapper _mapper;
        //public ShareNoteController(IMapper mapper, MyDBContext my_context)
        //{
        //    _context = my_context;
        //    _mapper = mapper;
        //}
        public ShareNoteController()
        {
            
        }
        public IActionResult Index()
        {
            return Ok("Sharing is On");
        }


        [HttpPost("share/{noteId}")]
        public IActionResult SharingNote(int noteId, ShareNote sharedNote)
        {
            // Validate request and permissions, generate unique link, and store it in the database
            // Return the unique link to the user
            return Ok(new { link = "link" });
        }
    }
}
