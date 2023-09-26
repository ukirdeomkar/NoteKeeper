using AutoMapper;
using NoteKeeper.Dtos;
using NoteKeeper.Models;

namespace NoteKeeper.Profiles
{
    public class NoteProfile : Profile
    {
      
            public NoteProfile()
            {
                CreateMap<Note, NoteDto>();
                CreateMap<User, UserDto>();
                CreateMap<ShareNote, ShareNoteDto>();
            CreateMap<ShareNoteOtherUsers,ShareNoteOtherUserDto>();
            }
        
    }
}
