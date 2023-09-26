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
            var note = _context.Notes.Include(n=>n.User).ToList().Where(u => u.UserId.ToString() == userid);
            if (note == null)
            {
                return BadRequest("Not Found");
            }
            
            var noteDto = note.Select(note => _mapper.Map<NoteDto>(note)).Where(n=>n.Sharing>0);
            return Ok(noteDto);

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
                //Add Note Permission to db
                note.Sharing = shareNote.Sharing;
                note.Permission = shareNote.SharedPermission;

                if (userShared != null)
                {
                    var ShareUser = new ShareNoteOtherUsers();
                    ShareUser.NoteId = note.Id;
                    ShareUser.UserId = userShared.Id;
                    var check = _context.ShareNoteOtherUsers.SingleOrDefault(u => u.NoteId == ShareUser.NoteId && u.UserId == u.UserId );
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
    }
}
