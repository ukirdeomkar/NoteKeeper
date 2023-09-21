using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoteKeeper.Dtos;
using NoteKeeper.Models;
using System.Data.Entity;

namespace NoteKeeper.Controllers.Api
{
    [ApiController]
    [Route("/notekeeper/[controller]")]
    public class NoteController : ControllerBase
    {

        private MyDBContext _context;
        private readonly IMapper _mapper;

        public NoteController( IMapper mapper)   
        {
            _context = new MyDBContext();
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<NoteDto> GetNote()
        {
            var notes = _context.Notes.Include(n=>n.User).ToList();
            var noteDto = notes.Select(notes => _mapper.Map<NoteDto>(notes));
            return noteDto;
        }


        [HttpGet("{id}")]
        public Note GetNoteById(int id)
        {
            var note = _context.Notes.ToList().Single(u => u.Id == id);


            if (note == null)
            {
                Console.WriteLine("Not Found");
            }
            return note;

        }

        [HttpPost]
        public Note CreateNote(Note note)
        {
            //var note = _context.Notes.ToList();

            //Todo : Check Unique Email COndition 
            //Todo : AddPassword Hashing
            var DateAdded = DateTime.Now;
            note.DateAdded = DateAdded;

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
        public void UpdateNotes(int id, Note note)
        {
            // TODO : Use Hashing for Password and findout how to update Hashed Password ;
            // TODO : ADD extra field in user if required that can be updated
            var noteinDB = _context.Notes.Single(u => u.Id == id);
            if (note == null)
            {
                Console.WriteLine("Empty Note");
            }
            else
            {
                //userinDB.Password = user.Password;
                noteinDB.Title = note.Title;
                noteinDB.Description = note.Description;
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


    }
}
