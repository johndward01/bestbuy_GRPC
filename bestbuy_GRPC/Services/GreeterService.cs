using bestbuy_GRPC;
using bestbuy_GRPC.Models;
using Dapper;
using Grpc.Core;
using System.Data;

namespace bestbuy_GRPC.Services;
public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly IDbConnection _connection;

    public GreeterService(ILogger<GreeterService> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public IEnumerable<Product> GetProducts()
    {
        return _connection.Query<Product>("SELECT * FROM bestbuy.products;");
    }

    public Product GetProduct(int id)
    {
        return _connection.QuerySingleOrDefault<Product>("SELECT * FROM bestbuy.products WHERE ProductID = @id;", new { id = id });
    }

    public void InsertProduct(Product prod)
    {
        _connection.Execute("INSERT INTO bestbuy.products (Name, Price, CategoryID, OnSale, StockLevel)" +
                            " VALUES (@Name, @Price, @CategoryID, @OnSale, @StockLevel);",
        new { name = prod.Name, price = prod.Price, categoryID = prod.CategoryID, onSale = prod.OnSale, stockLevel = prod.StockLevel });
    }

    public void UpdateProduct(Product prod)
    {
        _connection.Execute("UPDATE bestbuy.products SET Name = @name, Price = @price, OnSale = @onSale, StockLevel = @stockLevel WHERE ProductID = @id",
            new { name = prod.Name, price = prod.Price, onSale = prod.OnSale, stockLevel = prod.StockLevel, id = prod.ProductID });
    }

    public void DeleteProduct(Product prod)
    {
        _connection.Execute("DELETE from bestbuy.products WHERE ProductID = @id", new { id = prod.ProductID });
        _connection.Execute("DELETE from bestbuy.reviews WHERE ProductID = @id", new { id = prod.ProductID });
        _connection.Execute("DELETE from bestbuy.sales WHERE ProductID = @id", new { id = prod.ProductID });
    }

    //public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    //{
    //    return Task.FromResult(new HelloReply
    //    {
    //        Message = "Hello " + request.Name
    //    });
    //}
}
