using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // Stores all recipes, looked up by their ID (like a phone book: ID -> Recipe)
    private readonly Dictionary<int, Recipe> _catalogue = new();

    // Keeps track of removed recipe IDs, most recent removal on top (like a stack of plates)
    private readonly Stack<int> _removedRecipes = new();

    // Ingredients the user wants to buy, in the order they were added
    private readonly List<string> _shoppingList = new();

    // Ordered list of recipe IDs to cook, in the order they'll be made
    private readonly LinkedList<int> _cookingPlan = new();

    // Holds the current recipe's instructions, done one at a time in order
    private readonly Queue<string> _pendingInstructions = new();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes is null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }

        foreach (var recipe in recipes)
        {
            if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title) || _catalogue.ContainsKey(recipe.Id))
            {
                throw new ArgumentException($"Invalid or duplicate recipe: Id={recipe.Id}, Title='{recipe.Title}'");
            }

            _catalogue[recipe.Id] = recipe;
        }
    }

    public int RecipeCount => _catalogue.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _pendingInstructions.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false; // invalid recipe data
        }

        if (_catalogue.ContainsKey(recipe.Id))
        {
            return false; // a recipe with this ID already exists — don't overwrite it
        }

        _catalogue[recipe.Id] = recipe;
        return true; // added successfully
    }

    public Recipe? FindRecipe(int recipeId)
    {
        _catalogue.TryGetValue(recipeId, out var recipe);
        return recipe;
    }

    public bool RemoveRecipe(int recipeId)
    {
        if (!_catalogue.ContainsKey(recipeId))
        {
            return false; // nothing to remove
        }

        if (_cookingPlan.Contains(recipeId))
        {
            return false; // can't remove a recipe that's currently planned
        }

        _catalogue.Remove(recipeId);
        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return 0; // recipe doesn't exist, nothing added
        }

        _shoppingList.AddRange(recipe.Ingredients);
        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList() => _shoppingList.AsReadOnly();

    public void ClearShoppingList() => _shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (FindRecipe(recipeId) is null)
        {
            return false; // recipe doesn't exist, can't add it to the plan
        }

        if (_cookingPlan.Contains(recipeId))
        {
            return false; // already in the plan, no duplicates allowed
        }

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        var wasRemoved = _cookingPlan.Remove(recipeId);
        if (wasRemoved)
        {
            _removedRecipes.Push(recipeId);
        }
        return wasRemoved;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false; // nothing to restore
        }

        var recipeId = _removedRecipes.Pop();

        if (!_catalogue.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false; // recipe no longer exists, or is already back in the plan
        }

        _cookingPlan.AddLast(recipeId); // put it back at the end of the plan
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return null; // nothing there to look at
        }

        return _removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan() => _cookingPlan.ToList();

    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is null || recipe.Instructions.Count == 0)
        {
            return false; // recipe doesn't exist, or has no instructions to cook
        }

        _pendingInstructions.Clear(); // clear any leftover instructions from a previous session
        foreach (var instruction in recipe.Instructions)
        {
            _pendingInstructions.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (_pendingInstructions.Count == 0)
        {
            return null;
        }

        return _pendingInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (_pendingInstructions.Count == 0)
        {
            return null;
        }

        return _pendingInstructions.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
