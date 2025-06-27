using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStackApp.Server.Data;
using FullStackApp.Server.Models;
using System.Diagnostics;
using Azure;

using FullStackApp.Server.Services;
using Sprache;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace FullStackApp.Server.Controllers.API
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly NewIdentityDbContext _context;

        private readonly IUserDataService _userDataService;
        
        public BookingsController(NewIdentityDbContext context, IUserDataService userDataService)
        {
            _context = context;
            _userDataService = userDataService;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookings>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }


        // GET: api/Bookings/Test
        [HttpGet("Test/{seat}")]
        public async Task<ActionResult<IEnumerable<Bookings>>> GetBookingsTest(string seats)
        {
            return await _context.Bookings.Where(m => m.SessionId == 1)
                .Where(m => m.SeatNumber == seats).ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookings>> GetBookings(int id)
        {
            var bookings = await _context.Bookings.FindAsync(id);

            if (bookings == null)
            {
                return NotFound();
            }

            return bookings;
        }


        // GET: api/Bookings/GetBookingsForMovies
        [Authorize(Roles = "Admin")]
        [HttpGet("GetBookingsForMovies")]
        public async Task<ActionResult<List<TicketCountDTO>>> GetBookingsForMovies()
        {
            System.FormattableString sql = $@"select m.title as 'movieTitle', count(*) as 'ticketCount' from(Bookings as b INNER JOIN SessionTimes as s ON (b.SessionId = s.Id)) INNER JOIN Movies as m ON(m.Id = s.MovieId) GROUP BY s.MovieId,m.title";

        var result = await _context.Database.SqlQuery<TicketCountDTO>(sql).ToListAsync();

            if (result != null)
            {
                return result;
            }
            
            else
                return null;

}

            // GET: api/Bookings/GetBookingsForUser/{userId}
            [Authorize]
        [HttpGet("GetBookingsForUser/{userId}")]
        public async Task<ActionResult<List<BookingsDTO>>> GetBookingsForUser(string userId)
        {

            // check if user is logged in user
            Boolean check = await _userDataService.ConfirmUser(HttpContext.User, userId);

            if (check)
            {


                System.FormattableString sql = $@"select
Movies.Title, StartDatetime, EndDatetime, SeatNumber from 
(Bookings INNER JOIN SessionTimes ON (Bookings.SessionId = SessionTimes.Id)) 
INNER JOIN Movies ON (Movies.Id = SessionTimes.MovieId) where UserId = {userId}";

                var result = await _context.Database.SqlQuery<BookingsDTO>(sql).ToListAsync();

                return result;
            }
            else
                return null;
        }

        // GET: api/Bookings/GetSeatsForSession/{sessionId}
        [HttpGet("GetSeatsForSession/{sessionId}")]
        public async Task<ActionResult<List<Int32>>> GetSeatsForSession(int sessionId)
        {

            // Get the seats currently booked for the movie session and return a list
            // of available seats

            var seatsBooked = await _context.Bookings.Where(m => m.SessionId == sessionId)
                .Select(bookings => bookings.SeatNumber).ToListAsync();

            var intList = new List<int>();

            foreach (var str in seatsBooked)
            {
                if (int.TryParse(str, out int num))
                {
                    intList.Add(num);
                }
            }

            // Define start and end seats
            int start = 1;
            int end = 100;

            List<int> numbers = new List<int>();

            for (int i = start; i <= end; i++)
            {
                numbers.Add(i);
            }

            if (seatsBooked == null)
            {
                return Ok(numbers);
            }
            else
            {
                // Reverse iterate over number list to safely remove elems

                for (int i = numbers.Count - 1; i >= 0; i--)
                {
                    if (intList.Contains(numbers[i]))
                    {
                        numbers.RemoveAt(i);
                    }
                }

                return Ok(numbers);

            }
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookings(int id, Bookings bookings)
        {
            if (id != bookings.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        /*
           {
               "seatNumber": "1,2,3",
               "sessionId": 1,
               "userId": "1234"
           }
           */
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        

    [HttpPost]
        public async Task<ActionResult<Bookings>> PostBookings(Bookings bookings)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var seatsAlreadyBooked = await _context.Bookings.Where(m => m.SessionId == bookings.SessionId)
                .Select(m => m.SeatNumber).ToListAsync();


            var intList = new List<int>();

            // strA "1,2,3"
            foreach (var strA in seatsAlreadyBooked)
            {
                if (strA.Length > 0)
                {
                    var strB = strA.Split(',');

                    foreach (var str in strB)
                    {
                        if (int.TryParse(str, out int num))
                        {
                            intList.Add(num);
                        }
                    }
                }
            }

            // Check and compare the request seat numbers with those that are already booked

            var commonSeats = new List<int>();

            var receivedNumbers2 = new List<int>();
           
            var receivedNumbers = bookings.SeatNumber.Split(',');
            
            foreach (var str in receivedNumbers)
            {
                if (int.TryParse(str, out int num))
                {
                    receivedNumbers2.Add(num);
                }
            }

            foreach (int num in receivedNumbers2)
            {
                if (intList.Contains(num))
                {
                    commonSeats.Add(num);
                }
            }

            
            if (seatsAlreadyBooked != null)
            {
                if (commonSeats.Count > 0)
                {
                    Debug.WriteLine("Seat already booked", intList);

                    string unavailableSeats = string.Join(", ", commonSeats);

                    var response = new CustomHttpResponse
                    {
                        message = $"Seats: \"{unavailableSeats}\" already taken"
                    };

                    return Ok(response);
                    
                }
            }

            try
            {
                _context.Bookings.Add(bookings);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Error:'{e}'");
                return BadRequest();
            }

       
            return CreatedAtAction("GetBookings", new { id = bookings.Id }, bookings);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookings(int id)
        {
            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(bookings);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingsExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

    
    }
    
}
