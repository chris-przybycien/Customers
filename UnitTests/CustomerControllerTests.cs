using AlintaCustomerApi.Controllers;
using AlintaCustomerApi.Database;
using AlintaCustomerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class CustomerControllerTests : IDisposable
    {
        private CustomerController _customerController;
        private DbContextOptions<CustomerDbContext> _options;
        private CustomerDbContext _customerDbContext;

        public CustomerControllerTests()
        {
            _options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: $"db_{Guid.NewGuid().ToString()}")
                .Options;
        }

        public void Dispose()
        {
            _customerDbContext.Dispose();
        }

        [Fact]
        public void GetCustomersAsync_ReturnsCustomers()
        {
            SetupCustomers(2);

            using (_customerDbContext = new CustomerDbContext(_options))
            {
                _customerController = new CustomerController(_customerDbContext);

                var result = _customerController.GetCustomersAsync();
                var value = result.Result.Value;

                Assert.True(value.Any());
                Assert.True(value.Count() == 2);
                Assert.IsAssignableFrom<IEnumerable<Customer>>(value);
            }
        }

        [Fact]
        public void AddCustomerAsync_AddsCustomer()
        {
            using (_customerDbContext = new CustomerDbContext(_options))
            {
                _customerController = new CustomerController(_customerDbContext);

                var newCustomer = new CustomerRequest
                {
                    FirstName = "foo",
                    LastName = "bar",
                    DateOfBirth = DateTime.Now.Date
                };

                var result = _customerController.AddCustomerAsync(newCustomer);
                var value = result.Result.Value;

                Assert.Equal(1, value.Id);
                Assert.Equal(newCustomer.FirstName, value.FirstName);
                Assert.Equal(newCustomer.LastName, value.LastName);
                Assert.Equal(newCustomer.DateOfBirth, value.DateOfBirth);
                Assert.IsAssignableFrom<Customer>(value);
            }
        }

        [Fact]
        public void EditCustomerAsync_UpdatesCustomer()
        {
            SetupCustomers(2);

            using (_customerDbContext = new CustomerDbContext(_options))
            {
                _customerController = new CustomerController(_customerDbContext);

                var updatedCustomer = new CustomerRequest
                {
                    FirstName = "Bar",
                    LastName = "Bash",
                    DateOfBirth = DateTime.Now.Date
                };

                var result = _customerController.EditCustomerAsync(1, updatedCustomer);
                var value = result.Result.Value;

                Assert.Equal(1, value.Id);
                Assert.Equal(updatedCustomer.FirstName, value.FirstName);
                Assert.Equal(updatedCustomer.LastName, value.LastName);
                Assert.Equal(updatedCustomer.DateOfBirth, value.DateOfBirth);
                Assert.IsAssignableFrom<Customer>(value);
            }
        }

        [Fact]
        public void DeleteCustomerAsync_DeletesCustomer()
        {
            SetupCustomers(2);

            using (_customerDbContext = new CustomerDbContext(_options))
            {
                _customerController = new CustomerController(_customerDbContext);

                var result = _customerController.DeleteCustomerAsync(1);
                var value = result.Result;

                Assert.True(_customerDbContext.Customers.Count() == 1);
                Assert.IsAssignableFrom<OkResult>(value);
            }
        }

        [Fact]
        public void SearchCustomersAsync_ReturnsSingleCustomer()
        {
            SetupCustomers(2);

            using (_customerDbContext = new CustomerDbContext(_options))
            {
                _customerController = new CustomerController(_customerDbContext);

                var result = _customerController.SearchCustomersAsync("foo_1");
                var value = result.Result.Value;

                Assert.Single(value);
                Assert.IsAssignableFrom<IEnumerable<Customer>>(value);
            }
        }

        private void SetupCustomers(int qty = 1)
        {
            using (_customerDbContext = new CustomerDbContext(_options))
            {
                for (int i = 1; i <= qty; i++)
                    _customerDbContext.Customers.Add(new Customer
                    {
                        Id = i,
                        FirstName = $"foo_{i}",
                        LastName = $"bar_{i}",
                        DateOfBirth = DateTime.Now.Date
                    });

                _customerDbContext.SaveChanges();
            }
        }
    }
}
