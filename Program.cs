
using Microsoft.Data.SqlClient;
using ECommerceManagementSystem.Repositories;
using ECommerceManagementSystem.Services;

CategoryRepository repository = new();
CategoryService service = new(repository);

while (true)
{
    Console.WriteLine("\n================================");
    Console.WriteLine("   CATEGORY MANAGEMENT");
    Console.WriteLine("================================");
    Console.WriteLine("1. Add Category");
    Console.WriteLine("2. View All Categories");
    Console.WriteLine("3. Search Category by ID");
    Console.WriteLine("4. Search Category by Name");
    Console.WriteLine("5. Update Category");
    Console.WriteLine("6. Deactivate Category");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                Console.Write("Category name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Description: ");
                string? description = Console.ReadLine();

                int newId = await service.AddAsync(name, description);

                Console.WriteLine(
                    $"Category added successfully. ID: {newId}");
                break;

            case "2":
                var categories = await service.GetAllAsync();

                if (categories.Count == 0)
                {
                    Console.WriteLine("No categories found.");
                    break;
                }

                foreach (var category in categories)
                {
                    Console.WriteLine(
                        $"ID: {category.CategoryId} | " +
                        $"Name: {category.CategoryName} | " +
                        $"Description: {category.Description ?? "-"} | " +
                        $"Active: {category.IsActive}");
                }
                break;

            case "3":
                Console.Write("Enter category ID: ");

                if (!int.TryParse(Console.ReadLine(), out int searchId))
                {
                    Console.WriteLine("Enter a valid number.");
                    break;
                }

                var found = await service.GetByIdAsync(searchId);

                if (found == null)
                    Console.WriteLine("Category not found.");
                else
                    Console.WriteLine(
                        $"ID: {found.CategoryId} | " +
                        $"Name: {found.CategoryName} | " +
                        $"Description: {found.Description ?? "-"} | " +
                        $"Active: {found.IsActive}");
                break;

            case "4":
                Console.Write("Enter category name: ");
                string keyword = Console.ReadLine() ?? "";

                var results = await service.SearchAsync(keyword);

                if (results.Count == 0)
                    Console.WriteLine("No matching categories found.");

                foreach (var category in results)
                {
                    Console.WriteLine(
                        $"{category.CategoryId} - " +
                        $"{category.CategoryName} " +
                        $"(Active: {category.IsActive})");
                }
                break;

            case "5":
                Console.Write("Enter category ID to update: ");

                if (!int.TryParse(Console.ReadLine(), out int updateId))
                {
                    Console.WriteLine("Enter a valid number.");
                    break;
                }

                Console.Write("New category name: ");
                string newName = Console.ReadLine() ?? "";

                Console.Write("New description: ");
                string? newDescription = Console.ReadLine();

                bool updated = await service.UpdateAsync(
                    updateId, newName, newDescription);

                Console.WriteLine(updated
                    ? "Category updated successfully."
                    : "Category not found or inactive.");
                break;

            case "6":
                Console.Write("Enter category ID to deactivate: ");

                if (!int.TryParse(Console.ReadLine(), out int deactivateId))
                {
                    Console.WriteLine("Enter a valid number.");
                    break;
                }

                bool deactivated =
                    await service.DeactivateAsync(deactivateId);

                Console.WriteLine(deactivated
                    ? "Category deactivated successfully."
                    : "Category not found, already inactive, or has active products.");
                break;

            case "0":
                Console.WriteLine("Exiting application...");
                return;

            default:
                Console.WriteLine("Invalid choice. Try again.");
                break;
        }
    }
    catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
    {
        Console.WriteLine(
            "A category with that name already exists.");
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Validation error: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
