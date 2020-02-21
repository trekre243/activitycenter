using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityEvent.Models
{
    public class ActPart
    {
        [Key]
        public int ActPartId { get; set; }

        public int UserId { get; set; }

        public int ActId { get; set; }

        public User Participant { get; set; }

        public Act Act { get; set; }
    }
}