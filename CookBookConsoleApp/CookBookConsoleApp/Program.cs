using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Recipe
{
    public string Name { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Instructions { get; set; }

    public Recipe(string name)
    {
        Name = name;
        Ingredients = new List<string>();
        Instructions = new List<string>();
    }
}

public class RecipeBook
{
    private List<Recipe> recipes;

    public RecipeBook()
    {
        recipes = new List<Recipe>();
        LoadRecipes();  // Load existing recipes when the RecipeBook is instantiated
    }

    public void AddRecipe(Recipe recipe)
    {
        recipes.Add(recipe);
        SaveRecipes();  // Save recipes after adding a new one
    }

    public void RemoveRecipe(Recipe recipe)
    {
        recipes.Remove(recipe);
        SaveRecipes();  // Save recipes after removing one
    }

    public List<Recipe> GetRecipes()
    {
        return recipes;
    }

    private void SaveRecipes()
    {
        using (StreamWriter writer = new StreamWriter("recipes.txt"))
        {
            foreach (Recipe recipe in recipes)
            {
                writer.WriteLine($"Name: {recipe.Name}");
                writer.WriteLine("Ingredients:");
                foreach (string ingredient in recipe.Ingredients)
                {
                    writer.WriteLine($"- {ingredient}");
                }
                writer.WriteLine("Instructions:");
                foreach (string instruction in recipe.Instructions)
                {
                    writer.WriteLine($"- {instruction}");
                }
                writer.WriteLine();
            }
        }
    }

    private void LoadRecipes()
    {
        if (File.Exists("recipes.txt"))
        {
            using (StreamReader reader = new StreamReader("recipes.txt"))
            {
                while (!reader.EndOfStream)
                {
                    string nameLine = reader.ReadLine();
                    if (nameLine == null)
                        break;

                    string name = nameLine.StartsWith("Name: ") ? nameLine.Substring(6) : nameLine;

                    Recipe recipe = new Recipe(name);

                    reader.ReadLine();  // Skip empty line
                    reader.ReadLine();  // Skip "Ingredients:"
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null || string.IsNullOrWhiteSpace(line))
                            break;
                        recipe.Ingredients.Add(line.Substring(2)); // Skip "- "
                    }
                    reader.ReadLine();  // Skip "Instructions:"
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null || string.IsNullOrWhiteSpace(line))
                            break;
                        recipe.Instructions.Add(line.Substring(2)); // Skip "- "
                    }
                    recipes.Add(recipe);
                }
            }
        }
    }

    static void Main(string[] args)
    {
        RecipeBook recipeBook = new RecipeBook();

        while (true)
        {
            Console.WriteLine("Recipe Book Menu:");
            Console.WriteLine("1. Add Recipe");
            Console.WriteLine("2. View Recipes");
            Console.WriteLine("3. Delete Recipe");
            Console.WriteLine("4. Exit");

            Console.WriteLine("Enter your choice:");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        AddRecipe(recipeBook);
                        break;
                    case 2:
                        ViewRecipes(recipeBook);
                        break;
                    case 3:
                        DeleteRecipe(recipeBook);
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

    }

    static void AddRecipe(RecipeBook recipeBook)
    {
        Console.WriteLine("Enter the name of the recipe (or press Enter to cancel):");
        string recipeName = Console.ReadLine();

        // Check if the user wants to cancel
        if (string.IsNullOrWhiteSpace(recipeName))
        {
            Console.WriteLine("Addition canceled. Press Enter to go back to the main menu.");
            Console.ReadLine();
            return;
        }

        // Continue with the recipe addition
        Recipe newRecipe = new Recipe(recipeName);

        Console.WriteLine("Enter the ingredients for the recipe (one per line, leave blank to finish):");
        while (true)
        {
            string ingredient = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ingredient))
            {
                break;
            }
            newRecipe.Ingredients.Add(ingredient);
        }

        Console.WriteLine("Enter the instructions for the recipe (one per line, leave blank to finish):");
        while (true)
        {
            string instruction = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(instruction))
            {
                break;
            }
            newRecipe.Instructions.Add(instruction);
        }

        recipeBook.AddRecipe(newRecipe);

        Console.WriteLine($"Recipe '{recipeName}' added successfully. Press Enter to go back to the main menu.");
        Console.ReadLine();
    }


    static void ViewRecipes(RecipeBook recipeBook)
    {
        List<Recipe> recipes = recipeBook.GetRecipes();

        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available.");
            return;
        }

        Console.WriteLine("Recipes:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }

        Console.WriteLine("Enter the number of the recipe to view details (0 to cancel):");
        int recipeNumber;
        if (int.TryParse(Console.ReadLine(), out recipeNumber) && recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            ViewRecipeDetails(recipes[recipeNumber - 1]);
        }
        else
        {
            Console.WriteLine("Invalid input. Returning to the main menu.");
        }
        Console.WriteLine("Press Enter to go back to the main menu.");
        Console.ReadLine();
    }



    static void ViewRecipeDetails(Recipe recipe)
    {
        Console.WriteLine($"Name: {recipe.Name}");
        Console.WriteLine("Ingredients:");
        foreach (string ingredient in recipe.Ingredients)
        {
            Console.WriteLine($"- {ingredient}");
        }
        Console.WriteLine("Instructions:");
        foreach (string instruction in recipe.Instructions)
        {
            Console.WriteLine($"- {instruction}");
        }
        Console.WriteLine();
    }

    static void DeleteRecipe(RecipeBook recipeBook)
    {
        List<Recipe> recipes = recipeBook.GetRecipes();

        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes available. Press Enter to go back to the main menu.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Select the recipe to delete (enter the number, 0 to cancel):");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }

        int recipeNumber;
        if (int.TryParse(Console.ReadLine(), out recipeNumber) && recipeNumber >= 1 && recipeNumber <= recipes.Count)
        {
            Recipe recipeToDelete = recipes[recipeNumber - 1];

            Console.WriteLine($"Are you sure you want to delete the recipe '{recipeToDelete.Name}'? (Y/N)");
            string confirmation = Console.ReadLine();

            if (confirmation?.Trim().ToUpper() == "Y")
            {
                recipeBook.RemoveRecipe(recipeToDelete);
                Console.WriteLine($"Recipe '{recipeToDelete.Name}' deleted successfully. Press Enter to go back to the main menu.");
            }
            else
            {
                Console.WriteLine("Deletion canceled. Press Enter to go back to the main menu.");
            }
        }
        else if (recipeNumber == 0)
        {
            Console.WriteLine("Deletion canceled. Press Enter to go back to the main menu.");
        }
        else
        {
            Console.WriteLine("Invalid input. Returning to the main menu. Press Enter to continue.");
        }

        Console.ReadLine();
    }

}