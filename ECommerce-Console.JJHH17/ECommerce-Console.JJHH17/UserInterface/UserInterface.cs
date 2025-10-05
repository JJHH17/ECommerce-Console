using ECommerce_Console.JJHH17.UserInterface.SubMenus;
using Spectre.Console;
using System.Linq.Expressions;
using System.Net.Http.Json;
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
                        ProductMenu.ProductMenuOptions();
                        Console.Clear();
                        break;

                    case MenuOptions.Categories:
                        CategoryMenu.CategoryOptions();
                        Console.Clear();
                        break;

                    case MenuOptions.Sales:
                        SaleMenu();
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

        enum SaleMenuOptions
        {
            ViewAllSales,
            ViewSaleById,
            AddSale,
            ExitToMenu
        }

        public static void SaleMenu()
        {
            Console.WriteLine();

            bool SaleLoop = true;
            while (SaleLoop)
            {
                Console.Clear();
                AnsiConsole.MarkupLine("[blue]Sales Menu[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<SaleMenuOptions>()
                    .Title("Please select an option")
                    .AddChoices(Enum.GetValues<SaleMenuOptions>()));

                switch (choice)
                {
                    case SaleMenuOptions.ViewAllSales:
                        ViewAllSales();
                        Console.WriteLine("Enter any key to continue");
                        Console.ReadKey();
                        break;

                    case SaleMenuOptions.ViewSaleById:
                        ViewSaleById();
                        Console.WriteLine("Enter any key to continue");
                        Console.ReadKey();
                        break;

                    case SaleMenuOptions.AddSale:
                        Console.WriteLine("Feature coming soon");
                        Console.ReadKey();
                        break;

                    case SaleMenuOptions.ExitToMenu:
                        AnsiConsole.MarkupLine("[blue]Enter any key to exit to main menu...[/]");
                        Console.ReadKey();
                        SaleLoop = false;
                        break;
                }
            }
        }

        public async static void ViewAllSales()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Printing all sales[/]");

            using HttpClient client = new HttpClient();

            var table = new Table();
            table.AddColumn("Sale ID");
            table.AddColumn("Total Price");
            table.AddColumn("Item Quantity");

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7054/api/Sales");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                foreach (var sale in System.Text.Json.JsonDocument.Parse(responseBody).RootElement.EnumerateArray())
                {
                    string saleID = sale.GetProperty("saleId").ToString();
                    string totalPrice = sale.GetProperty("itemCount").ToString();
                    string itemQuantity = sale.GetProperty("salePrice").ToString();

                    table.AddRow(saleID, totalPrice, itemQuantity);
                }

                AnsiConsole.Write(table);
            }
            catch (HttpRequestException e)
            {
                AnsiConsole.MarkupLine($"[red]Request error: {e.Message}[/]");
            }
        }

        public async static void ViewSaleById()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Print a sale by its ID[/]");
            int inputId;

            while (true)
            {
                Console.WriteLine("Enter the ID of the sale that you want to view");
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

            var productInfo = new Table();
            productInfo.AddColumn("Product ID");
            productInfo.AddColumn("Product Name");
            productInfo.AddColumn("Product Price");

            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://localhost:7054/api/Sales/{inputId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    AnsiConsole.MarkupLine($"[red]Sale ID: {inputId} not found[/]");
                    return;
                }

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                // Product info data
                if (root.TryGetProperty("products", out var productsElem) && productsElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var prod in productsElem.EnumerateArray())
                    {
                        var prodId = prod.GetProperty("productId").ToString();
                        var prodName = prod.GetProperty("productName").ToString();
                        var prodPrice = prod.GetProperty("price").ToString();

                        productInfo.AddRow(prodId, prodName, prodPrice);
                    }

                    if (productInfo.Rows.Count > 0)
                    {
                        AnsiConsole.WriteLine();
                        AnsiConsole.MarkupLine("[blue]Products[/]");
                        AnsiConsole.Write(productInfo);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]No existing products found for this sale[/]");
                    }
                } 
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
