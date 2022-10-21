using Ididit.App.Data;
using Ididit.Data;
using Ididit.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.Persistence;

internal class DirectoryBackup
{
    private readonly IRepository _repository;

    public DirectoryBackup(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ImportData(NodeContent directory)
    {
        string name = directory.Name;
        NodeContent[] nodes = directory.Nodes;

        CategoryModel? root = _repository.CategoryList.FirstOrDefault(c => c.Name == name);

        if (root == null)
        {
            root = _repository.CreateCategory(name);

            await _repository.AddCategory(root);
        }

        await AddNodesToCategory(nodes, root);
    }

    private async Task AddNodesToCategory(NodeContent[] nodes, CategoryModel parent)
    {
        foreach (NodeContent node in nodes)
        {
            string name = node.Name;

            if (node.Kind == "file")
            {
                if (!name.EndsWith(".md"))
                    continue;

                name = name[0..^3];

                GoalModel? goal = parent.GoalList.FirstOrDefault(g => g.Name == name);

                if (goal == null)
                {
                    goal = parent.CreateGoal(_repository.NextGoalId, name);
                    goal.Details = node.Text;

                    await _repository.AddGoal(goal);
                }
                else
                {
                    goal.Details = node.Text;
                    await _repository.UpdateGoal(goal.Id);
                }

                TaskModel? task = null;

                foreach (string line in node.Text.Split(Environment.NewLine))
                {
                    if (task != null && line.StartsWith("- "))
                    {
                        task.DetailsText += line;

                        await _repository.UpdateTask(task.Id);
                    }
                    else
                    {
                        task = goal.CreateTask(_repository.NextTaskId, line);

                        await _repository.AddTask(task);
                    }
                }
            }
            else if (node.Kind == "directory")
            {
                if (name.StartsWith('.'))
                    continue;

                CategoryModel? category = parent.CategoryList.FirstOrDefault(c => c.Name == name);

                if (category == null)
                {
                    category = parent.CreateCategory(_repository.NextCategoryId, name);

                    await _repository.AddCategory(category);
                }

                await AddNodesToCategory(node.Nodes, category);
            }
        }
    }

    public NodeContent[] ExportData(IDataModel data)
    {
        List<NodeContent> nodes = new();

        foreach (CategoryModel category in data.CategoryList)
        {
            nodes.AddRange(AddNodes(category));
        }

        return nodes.ToArray();
    }

    private NodeContent[] AddNodes(CategoryModel parent)
    {
        List<NodeContent> nodes = new();

        foreach (GoalModel goal in parent.GoalList)
        {
            nodes.Add(new NodeContent()
            {
                Kind = "file",
                Name = goal.Name + ".md",
                Text = goal.Details
            });
        }

        foreach (CategoryModel category in parent.CategoryList)
        {
            NodeContent node = new()
            {
                Kind = "directory",
                Name = category.Name
            };

            nodes.Add(node);

            node.Nodes = AddNodes(category);
        }

        return nodes.ToArray();
    }
}
