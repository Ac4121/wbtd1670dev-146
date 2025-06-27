using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackApp.Server.Models
{
    public class SessionTimesDTO
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movies? Movies { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
