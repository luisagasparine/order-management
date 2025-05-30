using Dapper;
using Microsoft.Data.SqlClient;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using System.Data;

namespace OrderManagement.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnection _dbConnection;

        public OrderRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Order> GetById(int id)
        {
            var sql = @"
            SELECT * FROM Orders WHERE Id = @Id;
            SELECT * FROM OrderItems WHERE OrderId = @Id;
            ";

            using var multi = _dbConnection.QueryMultiple(sql, new { Id = id });

            var order = multi.Read<Order>().FirstOrDefault();
            if (order != null)
                order.OrderItems = multi.Read<OrderItem>().ToList();

            return order;
        }

        public async Task<int> GetOrderCountByCustomerId(int customerId)
        {
            string sql = @"
            SELECT COUNT(*) 
            FROM Orders 
            WHERE CustomerId = @CustomerId";

            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { CustomerId = customerId });
            return count;
        }

        public async Task<int> GetOrderItemCountByProductId(int productId)
        {
            string sql = @"
            SELECT COUNT(*) 
            FROM OrderItems 
            WHERE ProductId = @ProductId";

            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { ProductId = productId });
            return count;
        }

        public async Task<IEnumerable<Order>> GetByCustomerOrStatus(int? customerId = null, string status = null)
        {
            var orderDict = new Dictionary<int, Order>();

            var sql = @"
            SELECT 
                o.Id,
                o.CustomerId,
                o.OrderDate, 
                o.TotalAmount, 
                o.Status, 
                c.Id AS CustomerId, 
                c.Name, 
                c.Email, 
                c.Phone, 
                c.CreatedAt
            FROM 
            Orders o
            INNER JOIN 
                Customers c ON o.CustomerId = c.Id
            WHERE 1 = 1
            ";

            if (customerId.HasValue)
            {
                sql += " AND o.CustomerId = @CustomerId";
            }
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND o.Status = @Status";
            }

            var orders = _dbConnection.Query<Order, Customer, Order>(
                sql,
                (order, customer) =>
                {
                    order.Customer = customer;
                    return order;
                },
                new { CustomerId = customerId, Status = status },
                splitOn: "CustomerId" 
            ).ToList();

            return orders;
        }

        public async Task<Order> GetDetails(int id)
        {
            var sql = @"
            SELECT 
                o.Id, o.OrderDate, o.TotalAmount, o.Status,
                c.Id AS CustomerId, c.Name, c.Email, c.Phone,
                i.Quantity, i.UnitPrice,
                p.Id AS ProductId, p.Name
            FROM Orders o
            JOIN Customers c ON o.CustomerId = c.Id
            JOIN OrderItems i ON i.OrderId = o.Id
            JOIN Products p ON p.Id = i.ProductId
            WHERE o.Id = @Id";

            var orderDictionary = new Dictionary<int, Order>();

            var result = await _dbConnection.QueryAsync<Order, Customer, OrderItem, Product, Order>(
                sql,
                (order, customer, item, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Customer = customer;
                        orderEntry.OrderItems = new List<OrderItem>();
                        orderDictionary[order.Id] = orderEntry;
                    }

                    item.Product = product;
                    orderEntry.OrderItems.Add(item);

                    return orderEntry;
                },
                new { Id = id },
                splitOn: "CustomerId,Quantity,ProductId"
            );

            return result.FirstOrDefault();
        }

        public async Task<Order> Create(Order order)
        {
            var sqlOrder = @"
            INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status)
            VALUES (@CustomerId, @OrderDate, @TotalAmount, @Status);
            SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            order.Id = _dbConnection.QuerySingle<int>(sqlOrder, order);

            var sqlItem = @"
            INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
            VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
            ";

            foreach (var item in order.OrderItems)
            {
                item.OrderId = order.Id;
                _dbConnection.Execute(sqlItem, item);
            }

            return order;
        }

        public async Task<Order> Update(Order order)
        {
            var sql = @"
            UPDATE Orders
            SET CustomerId = @CustomerId,
                OrderDate = @OrderDate,
                TotalAmount = @TotalAmount,
                Status = @Status
            WHERE Id = @Id;
            ";

            _dbConnection.Execute(sql, order);
            return order;
        }


    }
}
