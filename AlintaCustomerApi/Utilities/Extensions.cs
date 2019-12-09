using AlintaCustomerApi.Models;

namespace AlintaCustomerApi.Utilities
{
    public static class Extensions
    {
        public static Customer ToCustomer(this CustomerRequest request, int id)
            => new Customer
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth
            };

        public static Customer UpdateCustomer(this CustomerRequest request, Customer customer)
        {
            if (customer == null) return null;

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.DateOfBirth = request.DateOfBirth;

            return customer;
        }
    }
}
