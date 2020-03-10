using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapstonePRS.Data;
using CapstonePRS.Models;

namespace CapstonePRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLinesController : ControllerBase {
        private readonly CapstonePRSContext _context;

        public RequestLinesController(CapstonePRSContext context) {
            _context = context;
        }

        private static void QuantityException(RequestLine requestLine) {
            if (requestLine.Quantity < 1) throw new Exception("Quantity must be greater than 0");
        }

        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLine() {
            return await _context.RequestLines.ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id) {
            var requestLine = await _context.RequestLines.FindAsync(id);

            if (requestLine == null) {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine) {
            if (id != requestLine.Id) {
                return BadRequest();
            }
            QuantityException(requestLine);
            _context.Entry(requestLine).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
                RecalcRequestTotal(requestLine.RequestId);
            } catch (DbUpdateConcurrencyException) {
                if (!RequestLineExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RequestLines
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine) {
            QuantityException(requestLine);

            _context.RequestLines.Add(requestLine);
            await _context.SaveChangesAsync();
            RecalcRequestTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestLine>> DeleteRequestLine(int id) {
            var requestLine = await _context.RequestLines.FindAsync(id);
            if (requestLine == null) {
                return NotFound();
            }

            _context.RequestLines.Remove(requestLine);
            await _context.SaveChangesAsync();
            RecalcRequestTotal(requestLine.RequestId);

            return requestLine;
        }

        private bool RequestLineExists(int id) {
            return _context.RequestLines.Any(e => e.Id == id);
        }

        private void RecalcRequestTotal(int requestId) {
            var request = _context.Requests.Find(requestId);
            var lines = _context.RequestLines.Where(rl => rl.RequestId == requestId);
            var total = lines.Sum(rl => rl.Quantity * rl.Product.Price);
            request.Total = total;
            _context.SaveChanges();
        }

    }
}
