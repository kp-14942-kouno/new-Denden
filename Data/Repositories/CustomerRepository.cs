using System.Data;
using Dapper;
using DenDen.Data.Database;
using DenDen.Models.DTOs;

namespace DenDen.Data.Repositories;

/// <summary>
/// 顧客マスタリポジトリの実装
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CustomerRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CustomerSearchResult>> SearchAsync(string? customerId, string? customerName, string? telNo)
    {
        using var connection = _connectionFactory.CreateCustomerConnection();
        connection.Open();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            conditions.Add("CustomerID LIKE @CustomerId");
            parameters.Add("CustomerId", $"%{customerId}%");
        }

        if (!string.IsNullOrWhiteSpace(customerName))
        {
            conditions.Add("(customer_name LIKE @CustomerName OR kana LIKE @CustomerName)");
            parameters.Add("CustomerName", $"%{customerName}%");
        }

        if (!string.IsNullOrWhiteSpace(telNo))
        {
            conditions.Add("tel_no LIKE @TelNo");
            parameters.Add("TelNo", $"%{telNo}%");
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        var sql = $@"
            SELECT
                CustomerID AS CustomerKey,
                customer_name AS CustomerName,
                tel_no AS TelNo,
                email AS Email
            FROM T_Customer
            {whereClause}
            ORDER BY CustomerID
            LIMIT 100";

        return await connection.QueryAsync<CustomerSearchResult>(sql, parameters);
    }

    /// <inheritdoc/>
    public async Task<CustomerSearchResult?> GetByIdAsync(string customerId)
    {
        using var connection = _connectionFactory.CreateCustomerConnection();
        connection.Open();

        var sql = @"
            SELECT
                CustomerID AS CustomerKey,
                customer_name AS CustomerName,
                tel_no AS TelNo,
                email AS Email
            FROM T_Customer
            WHERE CustomerID = @CustomerId";

        return await connection.QueryFirstOrDefaultAsync<CustomerSearchResult>(sql, new { CustomerId = customerId });
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object?>> GetCustomerDataAsync(string customerId)
    {
        using var connection = _connectionFactory.CreateCustomerConnection();
        connection.Open();

        var sql = "SELECT * FROM T_Customer WHERE CustomerID = @CustomerId";
        var result = await connection.QueryFirstOrDefaultAsync(sql, new { CustomerId = customerId });

        if (result == null)
        {
            return new Dictionary<string, object?>();
        }

        var dict = new Dictionary<string, object?>();
        foreach (var prop in (IDictionary<string, object>)result)
        {
            dict[prop.Key] = prop.Value;
        }
        return dict;
    }
}
