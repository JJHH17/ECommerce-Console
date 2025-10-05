using ECommerce_Console.JJHH17.UserInterface.SubMenus;
using Spectre.Console;
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
                        Console.WriteLine("Feature coming soon");
                        Console.ReadKey();
                        break;

                    case SaleMenuOptions.ViewSaleById:
                        Console.WriteLine("Feature coming soon");
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
    }
}
