using System.ComponentModel.DataAnnotations;

namespace FullStackApp.Server.Models
{
    public class Movies
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        public string? Genres { get; set; }

        public int? TicketsBoughtTotal { get; set; }

        public string? ESRBRating { get; set; }

        public string? Runtime { get; set; }

        public string? MovieImageFilename { get; set; }

    }
}
