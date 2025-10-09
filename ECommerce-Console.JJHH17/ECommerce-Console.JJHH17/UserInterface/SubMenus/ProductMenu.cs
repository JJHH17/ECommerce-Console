using Spectre.Console;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce_Console.JJHH17.UserInterface.SubMenus
{
    internal class ProductMenu
    {
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
                        AddNewProduct();
                        Console.WriteLine("Enter any key to continue");
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

        public static List<string> CreateProduct()
        {
            List<string> productInfo = new List<string>();
            productInfo.Add(NameProduct());
            productInfo.Add(ProductPrice());
            productInfo.Add(ProductCatId());

            return productInfo;
        }

        public static string NameProduct()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[blue]Create a new product[/]");
            string productName;

            while (true)
            {
                Console.WriteLine("Enter a product name");
                string inputName = Console.ReadLine();
                if (inputName.Length == 0 || inputName is null)
                {
                    Console.WriteLine("Please enter atleast 1 character");
                }
                else
                {
                    productName = inputName;
                    break;
                }
            }

            return productName;
        }

        public static string ProductPrice()
        {
            Console.Clear();
            decimal price;

            while (true)
            {
                AnsiConsole.MarkupLine("[blue]Enter the products price[/]");
                string priceString = Console.ReadLine();

                if (Decimal.TryParse(priceString, out price))
                {
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Please enter a valid price decimal[/]");
                }
            }

            return price.ToString();
        }

        public static string ProductCatId()
        {
            Console.Clear();
            int categoryId;
            CategoryMenu.ViewAllCategories();

            while (true)
            {
                AnsiConsole.MarkupLine("[blue]Enter the category ID that this product belongs to[/]");
                string catIdString = Console.ReadLine();

                if (int.TryParse(catIdString, out categoryId))
                {
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Please enter a valid numeric value[/]");
                }
            }

            return categoryId.ToString();
        }

        public async static Task AddNewProduct()
        {
            Console.Clear();
            List<string> newProductInfo = CreateProduct();

            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7054/api/") };

            var payload = new { name = newProductInfo[0], price = newProductInfo[1], categoryId = newProductInfo[2] };

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("Products", payload);

                if (response.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine("[blue]Product added![/]");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    AnsiConsole.MarkupLine($"[blue]Error! Product may already exist, or the category does not exist[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]Response failed: {response.StatusCode}[/]");
                }
            }
            catch (HttpRequestException e)
            {
                AnsiConsole.MarkupLine($"[red]A HTTP error occurred {e.Message}[/]");
            }
            catch (TaskCanceledException)
            {
                AnsiConsole.MarkupLine("[red]The response timed out. Please try again[/]");
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLine($"[red]An unexpected error occured: {e.Message}[/]");
            }
        }
    }
}
