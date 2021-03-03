using DemoECummerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoECummerApp.Controllers
{
    [EnableCors("mypolicy")]
    [Produces("application/json")]
    [Route("api/Customers")]
    public class CustomersController :  Controller
    {
        private readonly DemoDatabaseContext _context;
        public CustomersController(DemoDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Basic")]
        public IEnumerable<Customers> GetCustomers()
        {
            return _context.Customers;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public async Task<IActionResult> GetCustomers([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ident = User.Identity as ClaimsIdentity;
            var currentLoggeedInUserId = ident.Claims.FirstOrDefault(c
                => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentLoggeedInUserId != id.ToString())
            {
                return BadRequest("You are not authorized!");
            }

            var customers = await _context.Customers.SingleOrDefaultAsync(m => m.Id == id);
            if(customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public async Task<IActionResult> PutCustomer( Guid id, Customers customer)
        {
            if(id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersExists(id))
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

        [HttpPost]
        public async Task<IActionResult> PostCustomers([FromBody] Customers customers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_context.Customers.Any(x => x.Email == customers.Email))
            {
                ModelState.AddModelError("email", "User with mail id already exists");
                return BadRequest(ModelState);
            }
            _context.Customers.Add(customers);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomersExists (customers.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetCustomers", new { id = customers.Id }, customers);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Customers>> DeleteCustomer(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if(customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        private bool CustomersExists(Guid id)
        {
            return _context.Customers.Any(x => x.Id == id);
        }
    }
}
