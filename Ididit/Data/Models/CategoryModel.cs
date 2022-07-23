using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class CategoryModel
{
    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long? CategoryId { get; set; }

    public long? PreviousId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<CategoryModel> CategoryList = new();
    public List<GoalModel> GoalList = new();

    public CategoryModel CreateCategory(long id)
    {
        CategoryModel category = new()
        {
            Id = id,
            CategoryId = Id,
            PreviousId = CategoryList.Any() ? CategoryList.Last().Id : null
        };

        CategoryList.Add(category);

        return category;
    }

    public GoalModel CreateGoal(long id)
    {
        GoalModel goal = new()
        {
            Id = id,
            CategoryId = Id,
            PreviousId = GoalList.Any() ? GoalList.Last().Id : null
        };

        GoalList.Add(goal);

        return goal;
    }

    public void RemoveCategory()
    {
        // TODO:: fix PreviousId
    }

    public void RemoveGoal()
    {
        // TODO:: fix PreviousId
    }

    public void OrderCategories()
    {
        List<CategoryModel> categoryList = new();
        long? previousId = null;

        while (CategoryList.Any())
        {
            CategoryModel category = CategoryList.Single(t => t.PreviousId == previousId);
            previousId = category.Id;
            categoryList.Add(category);
            CategoryList.Remove(category);
        }

        CategoryList = categoryList;
    }

    public void OrderGoals()
    {
        List<GoalModel> goalList = new();
        long? previousId = null;

        while (GoalList.Any())
        {
            GoalModel goal = GoalList.Single(t => t.PreviousId == previousId);
            previousId = goal.Id;
            goalList.Add(goal);
            GoalList.Remove(goal);
        }

        GoalList = goalList;
    }
}
