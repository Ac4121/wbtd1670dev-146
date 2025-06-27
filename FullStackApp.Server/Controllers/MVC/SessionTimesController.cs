using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FullStackApp.Server.Data;
using FullStackApp.Server.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authorization;

namespace FullStackApp.Server.Controllers.MVC
{
    [Authorize(Roles = "Admin")]
    [Route("staff/[controller]")]
    public class SessionTimesController : Controller
    {
        private readonly NewIdentityDbContext _context;

        public SessionTimesController(NewIdentityDbContext context)
        {
            _context = context;
        }

        // GET: api/SessionTimes/Index
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var sessions = _context.SessionTimes.Include(s => s.Movies);
            return View(await sessions.ToListAsync());
        }

        // GET: api/SessionTimes/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionTimes = await _context.SessionTimes
                .Include(s => s.Movies)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessionTimes == null)
            {
                return NotFound();
            }

            return View(sessionTimes);
        }

        // GET: SessionTimes/Create
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return View();
        }

        // POST: SessionTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("Id,MovieId,StartDatetime,EndDatetime")] SessionTimes sessionTimes)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Debug.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                // Check if there are overlapping session times before creating new session
                var conflictCheck = await _context.SessionTimes.Where(e => e.StartDatetime
                > sessionTimes.StartDatetime
                && e.StartDatetime < sessionTimes.EndDatetime).Where(e => e.EndDatetime
                > sessionTimes.StartDatetime
                && e.EndDatetime < sessionTimes.EndDatetime).FirstOrDefaultAsync();

                Debug.WriteLine("session check:", conflictCheck);

                if (conflictCheck == null)
                {
                    _context.Add(sessionTimes);
                    await _context.SaveChangesAsync();
                    // Add success message to user
                    ModelState.AddModelError("", "Added new session successfully");
                    ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
                    return View(sessionTimes);

                    //RedirectToAction(nameof(Index));
                }
                else { ModelState.AddModelError("", "Session already taken"); }

            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", sessionTimes.MovieId);
            return View(sessionTimes);
        }

        
        [HttpPost]
        [Route("Export")]
        // returns specifically a FileContentResult
        public async Task<IActionResult> Export([FromBody] IEnumerable<SessionExport> sessionTimes)
        {
            List<string> dataList = [];

            if (sessionTimes != null)
            {
           


                foreach (var item in sessionTimes)
                {
                    // Manually create the string representation using string.Format. csv format
                    string itemString = string.Format("{0},{1},{2}", item.Title, item.StartDatetime, item.EndDatetime);
                    dataList.Add(itemString);
                }

            
            }
            

            var fileContent = string.Join(Environment.NewLine, dataList);

            // Convert the string to a MemoryStream
            var memory = new MemoryStream();
            var writer = new StreamWriter(memory);
            writer.Write(fileContent);
            writer.Flush(); // Make sure the data is written to the stream
            memory.Position = 0; // Reset position to the beginning of the stream

            byte[] fileBytes = memory.ToArray();
            Debug.WriteLine("file read");

            // Return the file with the correct MIME type
            // Set the Content-Disposition header to force download
            Response.Headers.Append("Content-Type", "text/plain; charset=utf-8");
            Response.Headers.Append("Content-Disposition", "attachment; filename=\"example.txt\"");
            return File(fileBytes, "text/plain", "example.txt");
        }

        
            // GET: SessionTimes/Edit/5
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionTimes = await _context.SessionTimes.FindAsync(id);
            if (sessionTimes == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", sessionTimes.MovieId);
            return View(sessionTimes);
        }

        // POST: SessionTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,StartDatetime,EndDatetime")] SessionTimes sessionTimes)
        {
            if (id != sessionTimes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessionTimes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionTimesExists(sessionTimes.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", sessionTimes.MovieId);
            return View(sessionTimes);
        }

        // GET: SessionTimes/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessionTimes = await _context.SessionTimes
                .Include(s => s.Movies)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessionTimes == null)
            {
                return NotFound();
            }

            return View(sessionTimes);
        }

        // POST: SessionTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessionTimes = await _context.SessionTimes.FindAsync(id);
            if (sessionTimes != null)
            {
                _context.SessionTimes.Remove(sessionTimes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionTimesExists(int id)
        {
            return _context.SessionTimes.Any(e => e.Id == id);
        }
    }
}
