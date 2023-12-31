﻿using System.ComponentModel.DataAnnotations;

namespace NoteKeeper.Models
{
    public class User
    {
        public int Id { get; set; }

        
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

              
    }
}
