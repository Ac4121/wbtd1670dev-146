using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FullStackApp.Server.Models.User;
using Microsoft.AspNetCore.Identity;

namespace FullStackApp.Server.Models
{
    
    public class Bookings
    {
        [Key]
        public int Id { get; set; }

        public String SeatNumber { get; set; }

        public int SessionId { get; set; }

        [ForeignKey("SessionId")]
        public virtual SessionTimes? SessionTime { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Models.User.User? User { get; set; }
    }

    
}
