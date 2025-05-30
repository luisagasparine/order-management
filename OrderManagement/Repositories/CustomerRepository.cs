using Dapper;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using System.Data;

namespace OrderManagement.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnection _dbConnection;

        public CustomerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Customer> GetById(int id)
        {
            var query = "SELECT * FROM Customers WHERE Id = @Id";
            return _dbConnection.QueryFirstOrDefault<Customer>(query, new { Id = id });
        }

        public async Task<Customer> GetByEmail(string email)
        {
            var query = "SELECT * FROM Customers WHERE Email=@Email";

            return _dbConnection.QueryFirstOrDefault<Customer>(query, new { Email = email });
        }

        public async Task<IEnumerable<Customer>> GetByNameOrEmail(string name, string email)
        {
            var query = "SELECT * FROM Customers WHERE 1=1"; 

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND Name LIKE @Name";
            }

            if (!string.IsNullOrEmpty(email))
            {
                query += " AND Email LIKE @Email";
            }

            return _dbConnection.Query<Customer>(query, new { Name = "%" + name + "%", Email = "%" + email + "%" });
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            var query = "SELECT * FROM Customers WHERE 1=1";

            return _dbConnection.Query<Customer>(query);
        }

        public async Task Create(Customer customer)
        {
            var query = "INSERT INTO Customers (Name, Email, Phone, CreatedAt) VALUES (@Name, @Email, @Phone, @CreatedAt)";
            _dbConnection.Execute(query, customer);
        }

        public async Task Delete(int id)
        {
            var query = "DELETE FROM Customers WHERE Id = @Id";
            _dbConnection.Execute(query, new { Id = id });
        }

        public async Task<Customer> Update(Customer customer)
        {
            var query = @"
            UPDATE Customers
            SET 
                Name = @Name,
                Email = @Email,
                Phone = @Phone
            WHERE Id = @Id";

            var rowsAffected = _dbConnection.Execute(query, customer);

            if (rowsAffected > 0)
            {
                return await GetById(customer.Id);
            }

            return null; 
        }

    }
}
