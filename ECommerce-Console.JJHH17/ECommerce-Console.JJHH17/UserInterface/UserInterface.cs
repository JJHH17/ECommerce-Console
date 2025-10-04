using System;
using Spectre.Console;
using System.Text.Json;

namespace ECommerce_Console.JJHH17.UserInterface
{
    public class UserInterface
    {
        public static void Menu()
        {
            Console.Clear();
            WelcomeMessage();

            bool running = true;
            while (running)
            {
                Console.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<MenuOptions>()
                    .Title("Please select an option")
                    .AddChoices(Enum.GetValues<MenuOptions>()));

                switch (choice)
                {
                    case MenuOptions.Products:
                        Console.WriteLine("Feature coming soon");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case MenuOptions.Categories:
                        CategoryMenu();
                        Console.Clear();
                        break;

                    case MenuOptions.Sales:
                        Console.WriteLine("Feature coming soon");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case MenuOptions.Quit:
                        AnsiConsole.MarkupLine("[blue]Thank you for using the ECommerce application! Please enter any key to exit[/]");
                        Console.ReadKey();
                        Console.Clear();
                        running = false;
                        break;
                }
            }
        }

        enum MenuOptions
        {
            Products,
            Categories,
            Sales,
            Quit
        }

        public static void WelcomeMessage()
        {
            AnsiConsole.MarkupLine("[darkslategray1]Welcome to the ECommerce GUI![/]");
            AnsiConsole.MarkupLine("[darkslategray1]From here, you can create a sale of products, add a new product, or add a new product category[/]");
        }

        enum CategoryMenuOptions
        {
            ViewAllCategories,
            ViewCategoryById,
            AddCategory,
            ExitToMenu
        }

        public static void CategoryMenu()
        {
            Console.WriteLine();

            bool CategoryLoop = true;
            while (CategoryLoop)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[blue]Category Menu[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<CategoryMenuOptions>()
                    .Title("Please select an option")
                    .AddChoices(Enum.GetValues<CategoryMenuOptions>()));

                switch (choice)
                {
                    case CategoryMenuOptions.ViewAllCategories:
                        ViewAllCategories();
                        Console.WriteLine("\nEnter any key to continue...");
                        Console.ReadKey();
                        break;

                    case CategoryMenuOptions.ViewCategoryById:
                        Console.WriteLine("Feature coming soon...");
                        Console.ReadKey();
                        break;

                    case CategoryMenuOptions.AddCategory:
                        Console.WriteLine("Feature coming soon...");
                        Console.ReadKey();
                        break;

                    case CategoryMenuOptions.ExitToMenu:
                        AnsiConsole.MarkupLine("[blue]Enter any key to exit to main menu...[/]");
                        Console.ReadKey();
                        CategoryLoop = false;
                        break;
                }
            }
        }

        public async static void ViewAllCategories()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Printing all categories[/]");

            using HttpClient client = new HttpClient();

            var table = new Table();
            table.AddColumn("Category ID");
            table.AddColumn("Category Name");
            table.AddColumn("Product Quantity");

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7054/api/Categories");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                foreach(var category in System.Text.Json.JsonDocument.Parse(responseBody).RootElement.EnumerateArray())
                {
                    // Collecting category info
                    string categoryId = category.GetProperty("categoryId").ToString();
                    string categoryName = category.GetProperty("categoryName").ToString();

                    // For product quantity count
                    int productCount = 0;
                    if (category.TryGetProperty("products", out JsonElement productsElement) &&
                        productsElement.ValueKind == JsonValueKind.Array)
                    {
                        productCount = productsElement.GetArrayLength();
                    }

                    table.AddRow(categoryId, categoryName, productCount.ToString());
                }
                AnsiConsole.Write(table);
            }

            catch (HttpRequestException e)
            {
                AnsiConsole.MarkupLine($"[red]Request error: {e.Message}[/]");
            }
        }
    }
}
