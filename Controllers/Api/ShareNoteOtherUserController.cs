using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Net;
using System.Runtime.Intrinsics.X86;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/shareuser")]
    [EnableCors("AllowAnyCorsPolicy")]
    public class ShareNoteOtherUserController : ControllerBase
    {
        private MyDBContext _context;
        private readonly IMapper _mapper;
        public ShareNoteOtherUserController(IMapper mapper, MyDBContext my_context)
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
            var notes = _context.Notes.Include(n => n.User).ToList();
            var noteDto = notes.Select(notes => _mapper.Map<NoteDto>(notes)).Where(n => n.Sharing > 0);
            return noteDto;
        }


        [HttpGet("shared")]
        [Authorize]
        public IActionResult GetSharedNoteByUserId()
        {
            var userid = User.FindFirst("id")?.Value;
            var user = _context.Users.ToList().SingleOrDefault(u => u.Id.ToString() == userid);

            if (user == null)
            {
                return BadRequest(new { error = "Not Found" });
            }
            var relations = _context.ShareNoteOtherUsers.Where(u => u.UserId == user.Id).ToList();
            var noteIds = relations.Select(relation => relation.NoteId).ToList();
            var notes = _context.Notes.Include(n => n.User).Where(n => noteIds.Contains(n.Id) && n.Permission > 0).ToList();

            var noteDto = notes.Select(notes => _mapper.Map<NoteDto>(notes)).Where(n => n.Sharing > 0);
            return Ok(notes);

        }

        [Authorize]
        [HttpGet("{uniqueLink}")]
        public IActionResult ViewNote(Guid uniqueLink)
        {
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null)
            {
                return NotFound(new{error="Not Found"});
            }
            var check = _context.ShareNoteOtherUsers.SingleOrDefault(n=>n.NoteId == uniqueLink  && n.UserId.ToString()== userid);
            if (check == null)
            {
                return BadRequest(new {error= "You do not have access to this Note" });
            }
            var noteDto = _mapper.Map<NoteDto>(note);
            return Ok(noteDto);
        }


        [Authorize]
        [HttpPost("{noteId}")]
        public IActionResult SharingNote(Guid noteId, ShareNote shareNote)
        {
            bool success= false;
            if (shareNote == null)
            {
                return BadRequest(new { error = "Provide Proper Share Request" });
            }
            var note = _context.Notes.SingleOrDefault(n => n.Id == noteId);
            if (note == null)
            {
                return NotFound();
            }
            var userPerformingDelete = _context.Users.SingleOrDefault(u => u.Id == note.UserId);
            if (userPerformingDelete == null)
            {
                return BadRequest(new { error = "Note or User Not Found" });
            }

            var userWithAccess = User.FindFirst("id")?.Value;


            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest(new {error= "Unauthorised Action" });
            }
            // Generate unique link from guid of note
            shareNote.UniqueLink = note.Id;
            //Add Note Permission to db
            note.Sharing = shareNote.Sharing;
            note.Permission = shareNote.SharedPermission;   
            _context.SaveChanges();
            success= true;
            // Return the unique link to the user
            return Ok(new { link = shareNote.UniqueLink, permission = note.Permission , sharing = note.Sharing , success });
        }

        [Authorize]
        [HttpPost("emails/{noteId}")]
        public IActionResult AddingEmailsToShareNotes(Guid noteId,[FromBody] ShareToEmails shareToEmails)
        {
            bool success = false;
            var note = _context.Notes.SingleOrDefault(n => n.Id == noteId);
            if (note == null)
            {
                return NotFound(new {error="Not Found"});
            }

            var userWithAccess = User.FindFirst("id")?.Value;
            var userWithAccessValue = _context.Users.SingleOrDefault(u => u.Id.ToString() == userWithAccess);

            if (shareToEmails.ShareNote.Sharing == ShareNote.sharedWithOtherUsers && shareToEmails.ShareNote.SharedPermission > ShareNote.notShared)
            {
                foreach (var email in shareToEmails.EmailList)
                {
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        return BadRequest(new { error = "Invalid email address in the list." });
                    }
                    #pragma warning disable
                    if (email == userWithAccessValue.Email)
                    {

                        return BadRequest(new { error = "You cannot share with yourself." });
                    }
                    #pragma warning restore

                    var userShared = _context.Users.SingleOrDefault(u => u.Email == email);
                    if (userShared == null)
                    {
                        return BadRequest(new {error= $"User with email {email} does not exist." });
                    }

                    var ShareUser = new ShareNoteOtherUsers
                    {
                        NoteId = note.Id,
                        UserId = userShared.Id
                    };

                    var check = _context.ShareNoteOtherUsers.SingleOrDefault(u => u.NoteId == ShareUser.NoteId && u.UserId == ShareUser.UserId);
                    if (check == null)
                    {
                        _context.ShareNoteOtherUsers.Add(ShareUser);
                    }
                    else
                    {
                        return BadRequest(new { error = "The Note is Already Shared with one or more users." });
                    }
                }
            }
            else
            {
                return BadRequest(new { error = "This Request cannot be completed to add emails" });
            }
            success = true;
            _context.SaveChanges();
            
            return Ok(new { link = note.Id, permission = note.Permission , success });
        }

        [Authorize]
        [HttpGet("emails/{noteId}")]
        public IActionResult GetSharedEmails(Guid noteId)
        {
            // Get the list of shared users' email addresses for the specified noteId
            var sharedEmails = _context.ShareNoteOtherUsers.Include(u=>u.User).Where(u=>u.NoteId == noteId ).Select(u => u.User.Email ).ToList();

            if (sharedEmails.Count == 0)
            {
                return NotFound(new { error = "No shared emails found for the specified note." });
            }

            return Ok(sharedEmails);
        }




        [Authorize]
        [HttpDelete("emails/{uniqueLink}")]
        public IActionResult RemoveNoteAccessFromUser(Guid uniqueLink , UserDto sharedUserEmail)
        {
            //var userid = User.FindFirst("id")?.Value;
            bool success = false;
            var sharedUser = _context.Users.SingleOrDefault(s=>s.Email ==  sharedUserEmail.Email);
            if(sharedUser == null)
            {
                return NotFound(new { error = "The user does not exist" });
            }
            var noteSharedUser = _context.ShareNoteOtherUsers.ToList().SingleOrDefault(s=>s.NoteId == uniqueLink && s.UserId == sharedUser.Id );
            //var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (noteSharedUser == null)
            {
                return NotFound(new { error = "The note was not shared with this user" });
            }
            _context.ShareNoteOtherUsers.Remove(noteSharedUser);
            _context.SaveChanges();
            success=true;

            return Ok(new { success});
        }

        [Authorize]
        [HttpPut("{uniqueLink}")]
        public IActionResult EditNote(Guid uniqueLink, Note noteInBody)
        {
            bool success = false;
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null)
            {
                return NotFound(new {error="Not Found"});
            }

            var relation = _context.ShareNoteOtherUsers.SingleOrDefault(n => n.NoteId == note.Id && n.UserId.ToString() == userid);
            if (relation == null)
            {
                return BadRequest(new { error = "You dont have permission to access to this note" });
            }
            if (note.Permission < ShareNote.editNote || note.Sharing == ShareNote.notShared)
            {
                return BadRequest(new { error = "Unauthorised Access : Cannot Edit this Note" });
            }
            note.Title = noteInBody.Title;
            note.Description = noteInBody.Description;
            
            _context.SaveChanges();
            success = true;
            return Ok(success);

        }


        [Authorize]
        [HttpDelete("{uniqueLink}")]
        public IActionResult DeleteNote(Guid uniqueLink)
        {
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null )
            {
                return NotFound(new {error="Not Found"});
            }
            var relation = _context.ShareNoteOtherUsers.SingleOrDefault(n => n.NoteId == note.Id && n.UserId.ToString() == userid);
            if (relation == null)
            {
                return BadRequest(new { error ="You dont have permission to access to this note" });
            }
            if (note.Permission < Note.deleteNote || note.Sharing == ShareNote.notShared)
            {
                return BadRequest(new {error= "Unauthorised Action : Dont Have Permission to delete" });
            }

            var sharedNotes = _context.ShareNoteOtherUsers.Where(n => n.NoteId == uniqueLink);

            _context.ShareNoteOtherUsers.RemoveRange(sharedNotes);
            _context.SaveChanges();

            _context.Notes.Remove(note);
            _context.SaveChanges();

            return Ok(new { success = "note deleted succesfuly" });
        }
    }
}
