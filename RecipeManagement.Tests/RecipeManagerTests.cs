using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

        [Fact]
    public void AddRecipe_RejectsDuplicateId()
    {
        var manager = CreateManager();
        var duplicate = new Recipe { Id = 10, Title = "Duplicate" };

        Assert.False(manager.AddRecipe(duplicate));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void FindRecipe_ReturnsNullForMissingId()
    {
        var manager = CreateManager();
        Assert.Null(manager.FindRecipe(999));
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseForMissingId()
    {
        var manager = CreateManager();
        Assert.False(manager.RemoveRecipe(999));
    }

    [Fact]
    public void ShoppingList_AddsIngredientsAndClears()
    {
        var manager = CreateManager();

        var added = manager.AddIngredientsToShoppingList(10);

        Assert.Equal(1, added);
        Assert.Equal(new[] { "1 apple" }, manager.GetShoppingList());

        manager.ClearShoppingList();
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void CookingPlan_RejectsDuplicateRecipe()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void RemovedRecipeStack_EmptyPeekAndRestoreHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void InstructionQueue_EmptyPeekAndCompleteHandledSafely()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void RemovingRecipeFromCatalogue_DoesNotAffectExistingCookingPlanEntry()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        manager.RemoveRecipe(10);

        Assert.Null(manager.FindRecipe(10));
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
