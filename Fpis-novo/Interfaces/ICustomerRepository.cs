using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Interfaces
{
    public interface ICustomerRepository
    {
        
        //bool CustomerAlreadyExists(string email);
        bool CustomerExists(int customerId);
        ICollection<Customer> GetCustomers();
        Customer GetCustomer(int customerId);
        Customer GetCustomerByEmail(string email);
        bool CreateCustomer(Customer customer);
        bool UpdateCustomer(Customer customer);
        bool DeleteCustomer(Customer customer);
        bool Save();
    }
}
