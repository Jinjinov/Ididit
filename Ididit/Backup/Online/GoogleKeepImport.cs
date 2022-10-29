using Ididit.Data;
using Ididit.Data.Model.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Backup.Online;

internal class GoogleKeepImport : IFileImport
{
    public string FileExtension => ".zip";

    private readonly IRepository _repository;

    public GoogleKeepImport(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ImportData(Stream stream)
    {
        CategoryModel category = _repository.Category;

        MemoryStream memoryStream = new();

        await stream.CopyToAsync(memoryStream);

        ZipArchive archive = new(memoryStream);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                await using Stream jsonStream = entry.Open();

                using StreamReader streamReader = new(jsonStream);

                string jsonText = await streamReader.ReadToEndAsync();

                GoogleKeepNote? googleKeepNote = JsonSerializer.Deserialize<GoogleKeepNote>(jsonText);

                if (googleKeepNote != null)
                {
                    GoalModel goal = category.CreateGoal(_repository.NextGoalId, googleKeepNote.Title);
                    goal.Details = googleKeepNote.TextContent;

                    await _repository.AddGoal(goal);

                    TaskModel? task = null;

                    foreach (string line in goal.Details.Split('\n'))
                    {
                        if (task != null && line.StartsWith("- "))
                        {
                            bool add = task.AddDetail(line);

                            if (add)
                                task.DetailsText += string.IsNullOrEmpty(task.DetailsText) ? line : Environment.NewLine + line;

                            await _repository.UpdateTask(task.Id);
                        }
                        else
                        {
                            task = goal.CreateTask(_repository.NextTaskId, line);

                            await _repository.AddTask(task);
                        }
                    }
                }
            }
        }
    }
}
