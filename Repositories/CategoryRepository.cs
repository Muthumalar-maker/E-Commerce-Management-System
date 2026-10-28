
using Microsoft.Data.SqlClient;
using System.Data;
using ECommerceManagementSystem.Helpers;
using ECommerceManagementSystem.Models;

namespace ECommerceManagementSystem.Repositories;

public class CategoryRepository
{
    public async Task<int> AddAsync(Category category)
    {
        const string sql = """
            INSERT INTO Categories (CategoryName, Description)
            VALUES (@CategoryName, @Description);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100)
            .Value = category.CategoryName;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, 300)
            .Value = (object?)category.Description ?? DBNull.Value;

        await connection.OpenAsync();

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<List<Category>> GetAllAsync()
    {
        const string sql = """
            SELECT CategoryId, CategoryName, Description, IsActive
            FROM Categories
            ORDER BY CategoryId;
            """;

        List<Category> categories = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(MapCategory(reader));
        }

        return categories;
    }

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        const string sql = """
            SELECT CategoryId, CategoryName, Description, IsActive
            FROM Categories
            WHERE CategoryId = @CategoryId;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@CategoryId", SqlDbType.Int)
            .Value = categoryId;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapCategory(reader);
    }

    public async Task<List<Category>> SearchAsync(string keyword)
    {
        const string sql = """
            SELECT CategoryId, CategoryName, Description, IsActive
            FROM Categories
            WHERE CategoryName LIKE @Keyword
            ORDER BY CategoryName;
            """;

        List<Category> categories = new();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 102)
            .Value = $"%{keyword}%";

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(MapCategory(reader));
        }

        return categories;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        const string sql = """
            UPDATE Categories
            SET CategoryName = @CategoryName,
                Description = @Description
            WHERE CategoryId = @CategoryId
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@CategoryId", SqlDbType.Int)
            .Value = category.CategoryId;

        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100)
            .Value = category.CategoryName;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, 300)
            .Value = (object?)category.Description ?? DBNull.Value;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() == 1;
    }

    public async Task<bool> DeactivateAsync(int categoryId)
    {
        const string sql = """
            UPDATE Categories
            SET IsActive = 0
            WHERE CategoryId = @CategoryId
              AND IsActive = 1
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM Products
                  WHERE Products.CategoryId = Categories.CategoryId
                    AND Products.IsActive = 1
              );
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@CategoryId", SqlDbType.Int)
            .Value = categoryId;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() == 1;
    }

    private static Category MapCategory(SqlDataReader reader)
    {
        return new Category
        {
            CategoryId = reader.GetInt32(0),
            CategoryName = reader.GetString(1),
            Description = reader.IsDBNull(2)
                ? null
                : reader.GetString(2),
            IsActive = reader.GetBoolean(3)
        };
    }
}
