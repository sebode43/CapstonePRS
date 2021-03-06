﻿using System;
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
    public class UsersController : ControllerBase {
        private readonly CapstonePRSContext _context;

        public UsersController(CapstonePRSContext context) {
            _context = context;
        }
  
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser() {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id) {
            var user = await _context.Users.FindAsync(id);

            if (user == null) {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user) {
            if (id != user.Id) {
                return BadRequest();
            }
            _context.Entry(user).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!UserExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user) {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id) {
            var user = await _context.Users.FindAsync(id);
            if (user == null) {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id) {
            return _context.Users.Any(e => e.Id == id);
        }

        //if errors occur may need to be async and will have to create a var with await in it

        [HttpGet(("login/{username}/{password}"))]
        public async Task<ActionResult<User>> Login(string username, string password) {
            try {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
                if (user == null) return NotFound();
                return user;
            } catch (ArgumentNullException ex) {
                throw new Exception("Cannot be null", ex);
            } catch (InvalidOperationException ex) {
                throw new Exception("Invalid username or password", ex);
            } catch (Exception) {
                throw;
            }

        }
        [HttpGet(("getpassword/{email}"))]
        public async Task<ActionResult<string>> GetPassword(string email) {
            try {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                return user.Password;
            } catch (ArgumentNullException ex) {
                throw new Exception("Cannot be null", ex);
            } catch (InvalidOperationException ex) {
                throw new Exception("Invalid entry, please make sure you have entered the correct information and try again", ex);
            } catch (Exception) {
                throw;
            }
        }
            

    }
}
