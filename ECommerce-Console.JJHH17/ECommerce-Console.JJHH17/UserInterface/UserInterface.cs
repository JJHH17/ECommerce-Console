using ECommerce_Console.JJHH17.UserInterface.SubMenus;
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
                        ProductMenuOptions();
                        Console.Clear();
                        break;

                    case MenuOptions.Categories:
                        CategoryMenu.CategoryOptions();
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

        enum ProductOptions
        {
            ViewAllProducts,
            ViewProductById,
            AddProduct,
            ExitToMenu
        }

        public static void ProductMenuOptions()
        {
            Console.WriteLine();

            bool ProductLoop = true;
            while (ProductLoop)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[blue]Product Menu[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<ProductOptions>()
                    .Title("Please select an option")
                    .AddChoices(Enum.GetValues<ProductOptions>()));

                switch (choice)
                {
                    case ProductOptions.ViewAllProducts:
                        ViewAllProducts();
                        Console.WriteLine("Enter any key to continue");
                        Console.ReadKey();
                        break;

                    case ProductOptions.ViewProductById:
                        ViewProductById();
                        Console.WriteLine("Enter any key to continue");
                        Console.ReadKey();
                        break;

                    case ProductOptions.AddProduct:
                        Console.WriteLine("Feature Coming soon...");
                        Console.ReadKey();
                        break;

                    case ProductOptions.ExitToMenu:
                        AnsiConsole.MarkupLine("[blue]Enter any key to exit to main menu...[/]");
                        Console.ReadKey();
                        ProductLoop = false;
                        break;
                }
            }
        }
        public async static void ViewAllProducts()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Printing all products[/]");

            using HttpClient client = new HttpClient();

            var table = new Table();
            table.AddColumn("Product ID");
            table.AddColumn("Product Name");
            table.AddColumn("Price");
            table.AddColumn("Category ID");
            table.AddColumn("Category Name");

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7054/api/Products");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                foreach (var product in System.Text.Json.JsonDocument.Parse(responseBody).RootElement.EnumerateArray())
                {
                    string productId = product.GetProperty("productId").ToString();
                    string productName = product.GetProperty("productName").ToString();
                    string price = product.GetProperty("price").ToString();
                    string categoryId = product.GetProperty("categoryId").ToString();
                    string categoryName = product.GetProperty("categoryName").ToString();

                    table.AddRow(productId, productName, price, categoryId, categoryName);
                }
                AnsiConsole.Write(table);
            }
            catch (HttpRequestException e)
            {
                AnsiConsole.MarkupLine($"[red]Request error: {e.Message}[/]");
            }
        }
        public async static void ViewProductById()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Print a product by its ID[/]");
            int inputId;

            while (true)
            {
                Console.WriteLine("Enter the ID of the product that you want to view");
                string stringInput = Console.ReadLine();

                if (int.TryParse(stringInput, out inputId))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid entry... Please enter a numeric value");
                }
            }
            using HttpClient client = new HttpClient();

            var table = new Table();
            table.AddColumn("Product ID");
            table.AddColumn("Product Name");
            table.AddColumn("Price");
            table.AddColumn("Category ID");
            table.AddColumn("Category Name");

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7054/api/Products/{inputId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    AnsiConsole.MarkupLine($"[red]Category ID: {inputId} not found[/]");
                    return;
                }

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                string productId = root.GetProperty("productId").ToString();
                string productName = root.GetProperty("productName").ToString();
                string price = root.GetProperty("price").ToString();
                string categoryId = root.GetProperty("categoryId").ToString();
                string categoryName = root.GetProperty("categoryName").ToString();

                table.AddRow(productId, productName, price, categoryId, categoryName);
                AnsiConsole.Write(table);
                }
                
            catch (HttpRequestException e)
                {
                    Console.Clear();
                    AnsiConsole.MarkupLine($"ID {inputId} not found, please try again, enter any key to continue...");
                    Console.ReadKey();
                }

            catch (Exception e)
                {
                    AnsiConsole.MarkupLine("[red]An error occurred. Please check the database and API connection.[/]");
                    AnsiConsole.MarkupLine("[red]Please also check the inputted value...[/]");
                    AnsiConsole.WriteLine(e.ToString());
            }
        }
    }
}
