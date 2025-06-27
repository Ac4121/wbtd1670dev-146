namespace FullStackApp.Server.Models
{
    public class BookingsDTO
    {
        public string Title { get; set; }
        public string SeatNumber { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public BookingsDTO(string title, String seatNumber, DateTime startDatetime, DateTime endDatetime)
        {
            Title = title;
            SeatNumber = seatNumber;
            StartDatetime = startDatetime;
            EndDatetime = endDatetime;

        }
    }
}
