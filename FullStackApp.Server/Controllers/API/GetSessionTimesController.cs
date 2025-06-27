using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FullStackApp.Server.Data;
using FullStackApp.Server.Models;

/*
 * This Controller handles the frontend logic for session times data
 */
namespace FullStackApp.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSessionTimesController : ControllerBase
    {
        private readonly NewIdentityDbContext _context;

        public GetSessionTimesController(NewIdentityDbContext context)
        {
            _context = context;
        }

        // Return in Session DTO format
        // GET: api/GetSessionTimes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionTimesDTO>>> GetSessionTimes()
        {
            return await _context.SessionTimes.Select(b =>
        new SessionTimesDTO()
        {
            Id = b.Id,
            MovieId = b.MovieId,
            Start = b.StartDatetime,
            End = b.EndDatetime
        }).ToListAsync();
        }

        // GET: api/GetSessionTimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionTimes>> GetSessionTimes(int id)
        {
            var sessionTimes = await _context.SessionTimes.FindAsync(id);

            if (sessionTimes == null)
            {
                return NotFound();
            }

            return sessionTimes;
        }

        // Return in Session DTO format
        // GET: api/GetSessionTimes/ByMovie/5
        [HttpGet("ByMovie/{id}")]
        public async Task<ActionResult<IEnumerable<SessionTimesDTO>>> GetSessionTimesByMovie(int id)
        {
            var sessionTimes = await _context.SessionTimes.Where(m => m.MovieId == id).Select(b =>
        new SessionTimesDTO()
        {
            Id = b.Id,
            MovieId = b.MovieId,
            Start = b.StartDatetime,
            End = b.EndDatetime
        }).ToListAsync();

            if (sessionTimes == null)
            {
                return NotFound();
            }

            return sessionTimes;
        }

        // PUT: api/GetSessionTimes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessionTimes(int id, SessionTimes sessionTimes)
        {
            if (id != sessionTimes.Id)
            {
                return BadRequest();
            }

            _context.Entry(sessionTimes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionTimesExists(id))
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

        // POST: api/GetSessionTimes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SessionTimes>> PostSessionTimes(SessionTimes sessionTimes)
        {
            _context.SessionTimes.Add(sessionTimes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSessionTimes", new { id = sessionTimes.Id }, sessionTimes);
        }

        // DELETE: api/GetSessionTimes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSessionTimes(int id)
        {
            var sessionTimes = await _context.SessionTimes.FindAsync(id);
            if (sessionTimes == null)
            {
                return NotFound();
            }

            _context.SessionTimes.Remove(sessionTimes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SessionTimesExists(int id)
        {
            return _context.SessionTimes.Any(e => e.Id == id);
        }
    }
}
