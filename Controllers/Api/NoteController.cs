using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using Microsoft.EntityFrameworkCore;


namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/[controller]")]
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


        [HttpGet("id/{id}")]
        public IEnumerable<NoteDto> GetNoteById(int id)
        {
            var note = _context.Notes.ToList().Where(u=>u.Id==id);
            var noteDto = note.Select(note => _mapper.Map<NoteDto>(note));

            if (note == null)
            {
                Console.WriteLine("Not Found");
            }
            return noteDto;

        }
        [HttpGet("user/{userid}")]
        public IEnumerable<NoteDto> GetNoteByUserId(int userid)
        {
            var note = _context.Notes.ToList().Where(u => u.UserId == userid);
            var noteDto = note.Select(note => _mapper.Map<NoteDto>(note));

            if (note == null)
            {
                Console.WriteLine("Not Found");
            }
            return noteDto;

        }

        [HttpPost]
        public Note CreateNote(Note note)
        {
            //var note = _context.Notes.ToList();

            //Todo : Check Unique Email COndition 
            //Todo : AddPassword Hashing
            var DateAdded = DateTime.Now;
            note.DateAdded = DateAdded;
            var user = _context.Users.SingleOrDefault(u => u.Id==note.UserId);
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
            return note;
        }

        [HttpPut("{id}")]
        public NoteDto UpdateNotes(int id, Note note)
        {
            // TODO : Use Hashing for Password and findout how to update Hashed Password ;
            // TODO : ADD extra field in user if required that can be updated
            var noteinDB = _context.Notes.Single(u => u.Id == id);
            if (note == null)
            {
                Console.WriteLine("Empty Note");
            }

                //userinDB.Password = user.Password;

            if(note.UserId != noteinDB.UserId)
            {
                Console.WriteLine("Unauthorized Action");
            }
                noteinDB.Title = note.Title;
                noteinDB.Description = note.Description;

                //try
                //{
                    _context.SaveChanges();
                var noteUpdated = _context.Notes.Single(u => u.Id == id);
                var noteDto =  _mapper.Map<NoteDto>(noteUpdated);
                return noteDto;

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            
            
        }


    }
}
