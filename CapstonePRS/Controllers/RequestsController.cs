using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapstonePRS.Data;
using CapstonePRS.Models;

namespace CapstonePRS.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase {
        private readonly CapstonePRSContext _context;

        public RequestsController(CapstonePRSContext context) {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequest() {
            return await _context.Requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id) {
            var request = await _context.Requests.FindAsync(id);

            if (request == null) {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request) {
            if (id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request) {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Request>> DeleteRequest(int id) {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return request;
        }

        private bool RequestExists(int id) {
            return _context.Requests.Any(e => e.Id == id);
        }

        public const string StatusNew = "NEW";
        public const string StatusEdit = "EDIT";
        public const string StatusReview = "REVIEW";
        public const string StatusApproved = "APPROVED";
        public const string StatusRejected = "REJECTED";

        public Task<IActionResult> Review(Request request) {
            if (request.Total <= 50) {
                request.Status = StatusApproved;
            } else {
                request.Status = StatusReview;
            }
            return PutRequest(request.Id, request);
        }
        public Task<IActionResult> Approve(Request request) {
            request.Status = StatusApproved;
            return PutRequest(request.Id, request);
        }
        public Task<IActionResult> Reject(Request request) {
            request.Status = StatusRejected;
            return PutRequest(request.Id, request);
        }
        public bool RejectReasonRequired(Request request) {
            var status = request.Status == StatusRejected;
            return request.RejectionReason != null;
        }
        public IEnumerable<Request> GetReviewsNotOwn(int userID) {
            return _context.Requests.Where(r => r.UserId != userID && r.Status == StatusReview).ToList();
        }
        public User SetUserId(int userId) {
            try {
                return _context.Users.SingleOrDefault(x => x.Id == userId);
            } catch (ArgumentNullException ex) {
                throw new Exception("Cannot be null", ex);
            }catch (InvalidOperationException ex) {
                throw new Exception("Invalid username or password", ex);
            }catch (Exception) {
                throw;
            }
        }

    }
}
