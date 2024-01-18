using FpisNovoAPI.Data;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FpisNovoAPI.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _dataContext;
        public CustomerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

    

        public bool CreateCustomer(Customer customer)
        {
            _dataContext.Customers.Add(customer);
            return Save();
        }

        /*public bool CustomerAlreadyExists(string email)
        {
            return _dataContext.Customers.Any(c => c.Email == email);
        }
        */
        public bool CustomerExists(int customerId)
        {
            return _dataContext.Customers.Any(c => c.CustomerId == customerId);
        }

        public bool DeleteCustomer(Customer customer)
        {
            _dataContext.Customers.Remove(customer);
            return Save();
        }

        public Customer GetCustomer(int customerId)
        {
            return _dataContext.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
        }
        public Customer GetCustomerByEmail(string email)
        {
            return _dataContext.Customers.Where(c => c.Email == email).FirstOrDefault();
        }
        public ICollection<Customer> GetCustomers()
        {
            return _dataContext.Customers.ToList();
        }


        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCustomer(Customer customer)
        {
            _dataContext.Customers.Update(customer);
            return Save();
        }
    }
}
