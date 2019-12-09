using AlintaCustomerApi.Database;
using AlintaCustomerApi.Models;
using AlintaCustomerApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlintaCustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private CustomerDbContext _dbContext;

        public CustomerController(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all customers from database
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersAsync()
        {
            IEnumerable<Customer> customers;

            try
            {
                customers = await _dbContext.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (customers.Any())
                return customers.ToList();
            else
                return NotFound();
        }

        /// <summary>
        /// Adds a new customer record to the database
        /// </summary>
        /// <param name="request">The customer request model</param>
        /// <returns>The newly added customer record</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Customer>> AddCustomerAsync([FromBody] CustomerRequest request)
        {
            Customer newCustomer;

            try
            {
                var nextCustId = _dbContext.Customers.Any() ? _dbContext.Customers.Max(c => c.Id) + 1 : 1;

                newCustomer = request.ToCustomer(nextCustId);

                _dbContext.Customers.Add(newCustomer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return newCustomer; 
        }

        /// <summary>
        /// Edit a customer record in the database
        /// </summary>
        /// <param name="id">The customer id</param>
        /// <param name="request">The customer request model</param>
        /// <returns>The updated customer record</returns>
        [HttpPut("{Id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Customer>> EditCustomerAsync(int id, [FromBody] CustomerRequest request)
        {
            Customer customer;

            try
            {
                customer = _dbContext.Customers.FirstOrDefault(c => c.Id == id);

                if (customer == null) return NotFound();

                customer = request.UpdateCustomer(customer);

                if (customer == null) return StatusCode((int)HttpStatusCode.InternalServerError);

                _dbContext.Customers.Update(customer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return customer;
        }

        /// <summary>
        /// Deletes a customer record from the database
        /// </summary>
        /// <param name="id">The customer id</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = _dbContext.Customers.FirstOrDefault(c => c.Id == id);

                if (customer == null)
                    return NotFound();

                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok();
        }

        /// <summary>
        /// Search for a customer using a keyword. Search will look at both first and last names for matches
        /// </summary>
        /// <param name="searchTerm">The name to search for</param>
        /// <returns>Customers</returns>
        [HttpGet("search/{searchTerm}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomersAsync(string searchTerm)
        {
            IEnumerable<Customer> customers;

            try
            {
                var query = _dbContext.Customers.AsQueryable()
                    .Where(q => q.FirstName.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
                        || q.LastName.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));

                customers = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (customers.Any())
                return customers.ToList();
            else
                return NotFound();
        }
    }
}