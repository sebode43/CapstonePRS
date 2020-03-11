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
    public class VendorsController : ControllerBase {
        private readonly CapstonePRSContext _context;

        public VendorsController(CapstonePRSContext context) {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendor() {
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id) {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null) {
                return NotFound();
            }

            return vendor;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor) {
            if (id != vendor.Id) {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!VendorExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Vendors
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor) {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.Id }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Vendor>> DeleteVendor(int id) {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return vendor;
        }

        private bool VendorExists(int id) {
            return _context.Vendors.Any(e => e.Id == id);
        }
        [HttpGet("PO/{name}")]
        public async Task<ActionResult<Vendor>> CreatePo(CapstonePRSContext context, string name, int id) {
            var PoJoin = from v in context.Vendors
                         join p in context.Products
                         on v.Id equals p.VendorId
                         where v.Id == id
                         join rl in context.RequestLines
                         on p.Id equals rl.ProductId
                         join r in context.Requests
                         on rl.RequestId equals r.Id
                         select new { Price = p.Price, Status = r.Status, Quantity = rl.Quantity, Name = v.Name };
            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }
            var approved = await PoJoin.Where(r => r.Status == "APPROVED").ToListAsync();
            var price = approved.Sum(a => (a.Price * a.Quantity) / 30);
            return _context.Vendors.SingleOrDefault(v => v.Name == name);
        }
        Dictionary<Product, int> GetPo =
        new Dictionary<Product, int>();

    }
}//dictionary - productid as key - find the quantity(data) - everytime read line item see if it is in dictionary
