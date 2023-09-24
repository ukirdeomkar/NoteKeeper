using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/[controller]")]
    [EnableCors("AllowLocalhost3000")]
    public class NoteController : ControllerBase
    {

        private MyDBContext _context;
        private readonly IMapper _mapper;

        public NoteController( IMapper mapper , MyDBContext my_context)   
        { 
            _context = my_context;
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<NoteDto> GetNote()
        {
            var notes = _context.Notes.Include(n=>n.User).ToList();
            var noteDto = notes.Select(notes => _mapper.Map<NoteDto>(notes));
            return noteDto;
        }

        [Authorize]
        [HttpGet("id/{id}")]
        public IActionResult GetNoteById(int id)
        {
            
            var note = _context.Notes.ToList().SingleOrDefault(u=>u.Id==id);
            if (note == null)
            {
                Console.WriteLine("Not Found");
            }
            var userIdwithAcess = User.FindFirst("id")?.Value;
            var userid = note.UserId;
            if (userIdwithAcess != userid.ToString())
            {
                return BadRequest("Unauthorised Access");
            }
            var noteDto = _mapper.Map<NoteDto>(note);
            return Ok(noteDto);

        }
        [HttpGet("user/notes")]

        [Authorize]
        public IActionResult GetNoteByUserId()
        {
            var userid = User.FindFirst("id")?.Value;
            //if(userIdwithAcess != userid.ToString())
            //{
            //    return BadRequest("Unauthorised Access");
            //}
            var note = _context.Notes.ToList().Where(u => u.UserId.ToString() == userid);
            if (note == null)
            {
                Console.WriteLine("Not Found");
            }
            var noteDto = note.Select(note => _mapper.Map<NoteDto>(note));
            return Ok(noteDto);

        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateNote(Note note)
        {
            //var note = _context.Notes.ToList();
            
            var userId = User.FindFirst("id")?.Value;
            var user = _context.Users.SingleOrDefault(u => u.Id.ToString() == userId);


            var DateAdded = DateTime.Now;
            note.DateAdded = DateAdded;
            note.UniqueId = Guid.NewGuid();
            

            note.User = user;
            
            _context.Notes.Add(note);
            //try
            //{
            _context.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            var noteDto = _mapper.Map<NoteDto>(note);
            return Ok(noteDto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNotes(int id, Note note)
        {
           
            var noteinDB = _context.Notes.SingleOrDefault(u => u.Id == id);
            if (note == null)
            {
                return BadRequest("Empty Note");
            }
            

            var userPerformingDelete = _context.Users.SingleOrDefault(u => u.Id == noteinDB.UserId);
            if (userPerformingDelete == null)
            {
                return BadRequest("Note or User Not Found");
            }

            var userWithAccess = User.FindFirst("id")?.Value;

            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest("Unauthorised Action");
            }


           
                noteinDB.Title = note.Title;
                noteinDB.Description = note.Description;

                //try
                //{
                    _context.SaveChanges();
                var noteUpdated = _context.Notes.Single(u => u.Id == id);
                var noteDto =  _mapper.Map<NoteDto>(noteUpdated);
                return Ok(noteDto);

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            
            
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteNote(int id)
        {
            var note = _context.Notes.SingleOrDefault(c => c.Id == id);
            if (note == null)
            {
                return BadRequest("Note Not Found");
            }

            var userPerformingDelete = _context.Users.Single(u => u.Id == note.UserId);
            var userWithAccess = User.FindFirst("id")?.Value;
            
            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest("Unauthorised Action");
            }

            // TODO : Check User Token while deleting from localStorage or someother way to confirm user identity



            _context.Notes.Remove(note);
            _context.SaveChanges();

            return Ok(new {Success = "note deleted succesfuly"});
        }

    }
}
