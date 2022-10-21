using Ididit.App.Data;
using Ididit.Data.Models;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Online;

internal class GoogleKeepImport
{
    private readonly IRepository _repository;

    public GoogleKeepImport(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ImportData(CategoryModel category, Stream stream)
    {
        MemoryStream memoryStream = new();

        await stream.CopyToAsync(memoryStream);

        ZipArchive archive = new(memoryStream);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.EndsWith(".json"))
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
                            task.DetailsText += line;

                            task.AddDetail(line);

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
