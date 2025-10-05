using Spectre.Console;
using ECommerce_Console.JJHH17.UserInterface.SubMenus;

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
                        Console.WriteLine("Feature coming soon...");
                        Console.ReadKey();
                        break;

                    case ProductOptions.ViewProductById:
                        Console.WriteLine("Feature coming soon...");
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
    }
}
