using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Net;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/[controller]")]
    [EnableCors("AllowAnyCorsPolicy")]
    public class NoteController : ControllerBase
    {

        private MyDBContext _context;
        private readonly IMapper _mapper;

        public NoteController( IMapper mapper , MyDBContext my_context)   
        { 
            _context = my_context;
            _mapper = mapper;
        }

        [HttpOptions]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
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
        public IActionResult GetNoteById(Guid id)
        {
            
            var note = _context.Notes.ToList().SingleOrDefault(u=>u.Id==id);
            if (note == null)
            {
                return BadRequest(new {error = "Not Found"});
            }
            var userIdwithAcess = User.FindFirst("id")?.Value;
            var userid = note.UserId;
            if (userIdwithAcess != userid.ToString())
            {
                return BadRequest(new { error = "Unauthorised Access" });
            }
            var noteDto = _mapper.Map<NoteDto>(note);
            return Ok(noteDto);

        }

        [HttpGet("user/notes")]

        [Authorize]
        public IActionResult GetNoteByUserId()
        {
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.ToList().Where(u => u.UserId.ToString() == userid);
            if (note == null)
            {
                return BadRequest(new {error= "Not Found" });
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
            note.Id = Guid.NewGuid();
            

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
        public IActionResult UpdateNotes(Guid id, Note note)
        {
           
            var noteinDB = _context.Notes.SingleOrDefault(u => u.Id == id);
            if (note == null || noteinDB == null)
            {
                return BadRequest(new {error= "Empty Note" });
            }
            

            var userPerformingDelete = _context.Users.SingleOrDefault(u => u.Id == noteinDB.UserId);
            if (userPerformingDelete == null)
            {
                return BadRequest(new {error= "Note or User Not Found" });
            }

            var userWithAccess = User.FindFirst("id")?.Value;

            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest(new { error="Unauthorised Action" });
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
        public IActionResult DeleteNote(Guid id)
        {
            var note = _context.Notes.SingleOrDefault(c => c.Id == id);
            if (note == null)
            {
                return BadRequest(new { error = "Note Not Found" });
            }

            var userPerformingDelete = _context.Users.Single(u => u.Id == note.UserId);
            var userWithAccess = User.FindFirst("id")?.Value;
            
            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest(new { error = "Unauthorised Action" });
            }

            var sharedNotes = _context.ShareNoteOtherUsers.Where(n => n.NoteId == id);


            // remove foreign keys containing noteid 
            _context.ShareNoteOtherUsers.RemoveRange(sharedNotes);
            _context.SaveChanges();

            // remove noteid
            _context.Notes.Remove(note);
            _context.SaveChanges();

            return Ok(new {success = "note deleted succesfuly"});
        }

    }
}
