
using ECommerceManagementSystem.Models;
using ECommerceManagementSystem.Repositories;

namespace ECommerceManagementSystem.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;

    public CategoryService(CategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(
        string name, string? description)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Category name is required.");

        if (name.Length > 100)
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.");

        if (description?.Length > 300)
            throw new ArgumentException(
                "Description cannot exceed 300 characters.");

        Category category = new()
        {
            CategoryName = name,
            Description = string.IsNullOrWhiteSpace(description)
                ? null : description.Trim()
        };

        return await _repository.AddAsync(category);
    }

    public Task<List<Category>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Category ID must be greater than zero.");

        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Category>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            throw new ArgumentException(
                "Enter a category name to search.");

        return await _repository.SearchAsync(keyword.Trim());
    }

    public async Task<bool> UpdateAsync(
        int id, string name, string? description)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid category ID.");

        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Category name is required.");

        if (name.Length > 100)
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.");

        if (description?.Length > 300)
            throw new ArgumentException(
                "Description cannot exceed 300 characters.");

        Category category = new()
        {
            CategoryId = id,
            CategoryName = name,
            Description = string.IsNullOrWhiteSpace(description)
                ? null : description.Trim()
        };

        return await _repository.UpdateAsync(category);
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid category ID.");

        return await _repository.DeactivateAsync(id);
    }
}
