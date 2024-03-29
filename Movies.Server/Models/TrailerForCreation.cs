﻿using System.ComponentModel.DataAnnotations;

namespace Movies.API.Models
{
    public class TrailerForCreation
    {
        [Required]
        public Guid MovieId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public byte[] Bytes { get; set; }
    }
}
