using AlintaCustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlintaCustomerApi.Database
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base (options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
