using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Net;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("notekeeper/[controller]")]
    [EnableCors("AllowAnyCorsPolicy")]
    public class ShareNoteController : Controller
    {
        private MyDBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public ShareNoteController(IMapper mapper,IConfiguration configuration, MyDBContext my_context)
        {
            _context = my_context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpOptions]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Authorize]
        [HttpPost("share/{noteId}")]
        public IActionResult SharingNote(Guid noteId, ShareNote shareNote)
        {
            if (shareNote == null)
            {
                return BadRequest(new {error= "Provide Proper Share Request" });
            }
            var note = _context.Notes.SingleOrDefault(n => n.Id == noteId);
            if (note == null)
            {
                return NotFound(new {error= "Not Found" });
            }
            var userPerformingDelete = _context.Users.SingleOrDefault(u => u.Id == note.UserId);
            if (userPerformingDelete == null)
            {
                return BadRequest(new {error= "Note or User Not Found" });
            }

            var userWithAccess = User.FindFirst("id")?.Value;

            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest(new { error="Unauthorised Action" });
            }

            // Generate unique link from guid of note
            shareNote.UniqueLink = note.Id;

            //Add Note Permission to db
            note.Sharing = shareNote.Sharing;
            note.Permission = shareNote.SharedPermission;
            _context.SaveChanges();

            var shareNoteDto = _mapper.Map<ShareNoteDto>(shareNote);

            // Return the unique link to the user
            return Ok(new { link = shareNote.UniqueLink , permission = note.Permission});
        }

        [AllowAnonymous]
        [HttpGet("{uniqueLink}")] 
        public IActionResult ViewNote(Guid uniqueLink) {
            
            var note = _context.Notes.SingleOrDefault(n=>n.Id == uniqueLink);
            if (note == null)
            {
                return NotFound(new {error="Not Found"});
            }
            if( note.Permission == Note.notShared  || note.Sharing != ShareNote.sharedAnonymously)
            {
                return BadRequest(new {error= "Unauthorised Access : Cannot view this note" });
            }
            var noteDto = _mapper.Map<NoteDto>(note);
            return Ok(noteDto );
        }

        [AllowAnonymous]
        [HttpPut("{uniqueLink}")]
        public IActionResult EditNote(Guid uniqueLink , Note noteInBody)
        {
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            

            if (note == null)
            {
                return NotFound();
            }
            if (note.Permission == Note.notShared || note.Permission < Note.editNote )
            {
                return BadRequest(new { error = "Unauthorised Access : Cannot Edit this Note" });
            }
            note.Title = noteInBody.Title;
            note.Description = noteInBody.Description;
            _context.SaveChanges();

            return Ok(note);

        }


        [AllowAnonymous]
        [HttpDelete("{uniqueLink}")]
        public IActionResult DeleteNote(Guid uniqueLink)
        {
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null)
            {
                return NotFound(new {error="Not Found"});
            }
            if(note.Permission < Note.deleteNote)
            {
                return BadRequest(new { error="Unauthorised Action : Dont Have Permission to delete" });
            }
            _context.Notes.Remove(note);
            _context.SaveChanges();

            return Ok(new { success = "note deleted succesfuly" });
        }

    }
}
