using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class CategoryModel
{
    [JsonIgnore]
    internal long Id { get; set; }
    [JsonIgnore]
    internal long? CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<CategoryModel> CategoryList = new();
    public List<GoalModel> GoalList = new();

    public CategoryModel CreateCategory()
    {
        CategoryModel category = new()
        {
            CategoryId = Id,
            Name = "Category " + CategoryList.Count
        };

        CategoryList.Add(category);

        return category;
    }

    public GoalModel CreateGoal()
    {
        GoalModel goal = new()
        {
            CategoryId = Id,
            Name = "Goal " + GoalList.Count
        };

        GoalList.Add(goal);

        return goal;
    }
}
