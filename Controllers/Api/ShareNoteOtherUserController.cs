using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteKeeper.Dtos;
using NoteKeeper.Models;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/shareuser")]
    [EnableCors("AllowLocalhost3000")]
    public class ShareNoteOtherUserController : ControllerBase
    {
        private MyDBContext _context;
        private readonly IMapper _mapper;
        public ShareNoteOtherUserController(IMapper mapper, MyDBContext my_context)
        {
            _context = my_context;
            _mapper = mapper;
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
            //if(userIdwithAcess != userid.ToString())
            //{
            //    return BadRequest("Unauthorised Access");
            //}
            var user = _context.Users.ToList().SingleOrDefault(u => u.Id.ToString() == userid);

            if (user == null)
            {
                return BadRequest("Not Found");
            }
            var relations = _context.ShareNoteOtherUsers.Where(u => u.UserId == user.Id).ToList();
            var noteIds = relations.Select(relation => relation.NoteId).ToList();
            var notes = _context.Notes.Include(n => n.User).Where(n => noteIds.Contains(n.Id)).ToList();
            var noteDto = notes.Select(notes => _mapper.Map<NoteDto>(notes)).Where(n => n.Sharing > 0);
            return Ok(notes);

        }


        [Authorize]
        [HttpPost("{noteId}")]
        public IActionResult SharingNote(Guid noteId, ShareNote shareNote)
        {
            if (shareNote == null)
            {
                return BadRequest("Provide Proper Share Request");
            }
            var note = _context.Notes.SingleOrDefault(n => n.Id == noteId);
            if (note == null)
            {
                return NotFound();
            }
            var userPerformingDelete = _context.Users.SingleOrDefault(u => u.Id == note.UserId);
            if (userPerformingDelete == null)
            {
                return BadRequest("Note or User Not Found");
            }

            var userWithAccess = User.FindFirst("id")?.Value;


            if (userPerformingDelete.Id.ToString() != userWithAccess)
            {
                return BadRequest("Unauthorised Action");
            }
            // Generate unique link from guid of note
            shareNote.UniqueLink = note.Id;
            //Add Note Permission to db
            note.Sharing = shareNote.Sharing;
            note.Permission = shareNote.SharedPermission;

            if (shareNote.Sharing == ShareNote.sharedWithOtherUsers)
            {
                var userWithAccessValue = _context.Users.SingleOrDefault(u => u.Id.ToString() == userWithAccess);
                if (shareNote.Email == null || shareNote.Email == userWithAccessValue.Email)
                {
                    return BadRequest("Please add valid email to share");
                }
                var userShared = _context.Users.SingleOrDefault(u => u.Email == shareNote.Email);
                if (userShared == null)
                {
                    return BadRequest("This User does not exist");
                }


                if (userShared != null)
                {
                    var ShareUser = new ShareNoteOtherUsers();
                    ShareUser.NoteId = note.Id;
                    ShareUser.UserId = userShared.Id;
                    //Console.WriteLine("The Shared USer is :"+ShareUser.NoteId + "   " + ShareUser.UserId);
                    var check = _context.ShareNoteOtherUsers.SingleOrDefault(u => u.NoteId == ShareUser.NoteId && u.UserId == ShareUser.UserId);
                    if (check == null)
                    {
                        _context.ShareNoteOtherUsers.Add(ShareUser);

                    }
                    else
                    {
                        return BadRequest("The Note is Already Shared");
                    }
                }


            }


            _context.SaveChanges();
            var shareNoteDto = _mapper.Map<ShareNoteDto>(shareNote);
            
            // Return the unique link to the user
            return Ok(new { link = shareNote.UniqueLink, permission = note.Permission });
        }


        [Authorize]
        [HttpDelete("user/{uniqueLink}")]
        public IActionResult RemoveNoteAccessFromUser(Guid uniqueLink , UserDto sharedUserEmail)
        {
            //var userid = User.FindFirst("id")?.Value;
            var sharedUser = _context.Users.SingleOrDefault(s=>s.Email ==  sharedUserEmail.Email);
            if(sharedUser == null)
            {
                return NotFound("The user does not exist");
            }
            var noteSharedUser = _context.ShareNoteOtherUsers.ToList().SingleOrDefault(s=>s.NoteId == uniqueLink && s.UserId == sharedUser.Id );
            //var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (noteSharedUser == null)
            {
                return NotFound("The note was not shared with this user");
            }
            _context.ShareNoteOtherUsers.Remove(noteSharedUser);
            _context.SaveChanges();

            return Ok(new { Success = "Acces removed for email "+ sharedUserEmail.Email });
        }

        [Authorize]
        [HttpPut("{uniqueLink}")]
        public IActionResult EditNote(Guid uniqueLink, Note noteInBody)
        {
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null)
            {
                return NotFound();
            }
            var relation = _context.ShareNoteOtherUsers.SingleOrDefault(n => n.NoteId == note.Id && n.UserId.ToString() == userid);
            if (relation == null)
            {
                return BadRequest("You dont have permission to access to this note");
            }
            if (note.Permission < ShareNote.editNote || note.Sharing == ShareNote.notShared)
            {
                return BadRequest("Unauthorised Access : Cannot Edit this Note");
            }
            note.Title = noteInBody.Title;
            note.Description = noteInBody.Description;
            _context.SaveChanges();

            return Ok(note);

        }


        [Authorize]
        [HttpDelete("{uniqueLink}")]
        public IActionResult DeleteNote(Guid uniqueLink)
        {
            var userid = User.FindFirst("id")?.Value;
            var note = _context.Notes.SingleOrDefault(n => n.Id == uniqueLink);
            if (note == null )
            {
                return NotFound();
            }
            var relation = _context.ShareNoteOtherUsers.SingleOrDefault(n => n.NoteId == note.Id && n.UserId.ToString() == userid);
            if (relation == null)
            {
                return BadRequest("You dont have permission to access to this note");
            }
            if (note.Permission < Note.deleteNote || note.Sharing == ShareNote.notShared)
            {
                return BadRequest("Unauthorised Action : Dont Have Permission to delete");
            }

            var sharedNotes = _context.ShareNoteOtherUsers.Where(n => n.NoteId == uniqueLink);
            Console.WriteLine("\nThe notes to be deleted have count :"+sharedNotes.Count()+"\n");

            _context.ShareNoteOtherUsers.RemoveRange(sharedNotes);
            _context.SaveChanges();

            _context.Notes.Remove(note);
            _context.SaveChanges();

            return Ok(new { Success = "note deleted succesfuly" });
        }
    }
}
