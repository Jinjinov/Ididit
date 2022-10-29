using System.Collections.Generic;
using System.Linq;

namespace Ididit.Data.Models;

public class CategoryModel
{
    public long Id { get; init; }

    public long? CategoryId { get; set; }

    public long? PreviousId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<CategoryModel> CategoryList = new();
    public List<GoalModel> GoalList = new();

    public CategoryModel CreateCategory(long id, string name)
    {
        CategoryModel category = new()
        {
            Id = id,
            CategoryId = Id,
            PreviousId = CategoryList.Any() ? CategoryList.Last().Id : null,
            Name = name
        };

        CategoryList.Add(category);

        return category;
    }

    public GoalModel CreateGoal(long id, string name)
    {
        GoalModel goal = new()
        {
            Id = id,
            CategoryId = Id,
            PreviousId = GoalList.Any() ? GoalList.Last().Id : null,
            Name = name,
            CreateTaskFromEachLine = true
        };

        GoalList.Add(goal);

        return goal;
    }

    public CategoryModel? RemoveCategory(CategoryModel category)
    {
        CategoryModel? changedCategory = null;

        int index = CategoryList.IndexOf(category);

        if (index < CategoryList.Count - 1)
        {
            changedCategory = CategoryList[index + 1];

            if (index > 0)
                changedCategory.PreviousId = CategoryList[index - 1].Id;
            else
                changedCategory.PreviousId = null;
        }

        CategoryList.Remove(category);

        return changedCategory;
    }

    public GoalModel? RemoveGoal(GoalModel goal)
    {
        GoalModel? changedGoal = null;

        int index = GoalList.IndexOf(goal);

        if (index < GoalList.Count - 1)
        {
            changedGoal = GoalList[index + 1];

            if (index > 0)
                changedGoal.PreviousId = GoalList[index - 1].Id;
            else
                changedGoal.PreviousId = null;
        }

        GoalList.Remove(goal);

        return changedGoal;
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
