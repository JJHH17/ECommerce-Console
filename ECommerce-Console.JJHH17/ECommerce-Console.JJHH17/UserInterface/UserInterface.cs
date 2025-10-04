using System;
using Spectre.Console;

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
                        Console.WriteLine("Feature coming soon...");
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
    }
}
