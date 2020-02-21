using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityEvent.Models
{
    public class Act
    {
        [Key]
        public int ActId { get; set; }

        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        public int Duration { get; set; }

        [Required]
        public string Units { get; set; }

        [Required]
        public string Description { get; set; }

        public int UserId { get; set; }

        public User Coordinator { get; set; }

        public List<ActPart> Participants { get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}