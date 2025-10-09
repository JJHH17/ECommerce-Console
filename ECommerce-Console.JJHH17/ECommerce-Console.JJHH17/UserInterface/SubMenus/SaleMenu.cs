using Spectre.Console;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce_Console.JJHH17.UserInterface.SubMenus
{
    internal class SaleMenu
    {
        enum SaleMenuOptions
        {
            ViewAllSales,
            ViewSaleById,
            AddSale,
            ExitToMenu
        }

        public static void SaleMenuUi()
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
                        AddNewSale();
                        Console.WriteLine("Enter any key to continue");
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

        public static List<int> ListProductIds()
        {
            var idList = new List<int>();

            Console.Clear();
            ProductMenu.ViewAllProducts();

            bool running = true;
            while (running)
            {
                int integerInput;
                AnsiConsole.MarkupLine("[blue]Enter a list of product IDs to purchase them[/]");
                AnsiConsole.MarkupLine("[blue]Enter an empty value to quit[/]");
                string inputString = Console.ReadLine();

                if (inputString.Length == 0)
                {
                    running = false;
                    break;
                }

                if (int.TryParse(inputString, out integerInput))
                {
                    idList.Add(integerInput);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Please enter a valid value[/]");
                }
            }

            return idList;
        }

        public static List<string> stringConversion()
        {
            List<int> toConvert = ListProductIds();
            List<string> converted = new List<string>();

            foreach (int item in toConvert)
            {
                converted.Add(item.ToString());
            }

            return converted;
        }

        public async static Task AddNewSale()
        {
            Console.Clear();
            List<string> payloadItems = stringConversion();

            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7054/api/") };
            var productIds = payloadItems.Select(int.Parse).ToList();

            var payload = new { productIds = productIds, };

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("Sales", payload);

                if (response.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine("[blue]Sale added![/]");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    AnsiConsole.MarkupLine($"[blue]Error! A category entered may not exist[/]");
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
