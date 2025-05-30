using Dapper;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace OrderManagement.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _dbConnection;

        public ProductRepository(IDbConnection dbConnection) {
            _dbConnection = dbConnection;
        }

        public async Task<Product> GetById(int id)
        {
            var query = "SELECT * FROM Products WHERE Id = @Id";
            return _dbConnection.QueryFirstOrDefault<Product>(query, new { Id = id });
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            var query = "SELECT * FROM Products WHERE 1=1";

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND Name LIKE @Name";
            }

            return _dbConnection.Query<Product>(query, new { Name = "%" + name + "%" });
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var query = "SELECT * FROM Products WHERE 1=1";

           
            return _dbConnection.Query<Product>(query);
        }

        public async Task Create(Product product)
        {
            var query = "INSERT INTO Products (Name, Description, Price, StockQuantity) VALUES (@Name, @Description, @Price, @StockQuantity)";
            _dbConnection.Execute(query, product);
        }

        public async Task Delete(int id)
        {
            var query = "DELETE FROM Products WHERE Id = @Id";
            _dbConnection.Execute(query, new { Id = id });
        }

        public async Task<Product> Update(Product product)
        {
            var query = @"
            UPDATE Products
            SET 
                Name = @Name,
                Description = @Description,
                Price = @Price,
                StockQuantity = @StockQuantity
            WHERE Id = @Id";

            var rowsAffected = _dbConnection.Execute(query, product);

            if (rowsAffected > 0)
            {
                return await GetById(product.Id);
            }
            return null; 
        }
    }
}
