using Ididit.Data;
using Ididit.Data.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ididit.Backup.Online;

internal class GoogleKeepImport : IFileImport, IFileToString
{
    public string FileExtension => ".zip";

    private readonly IRepository _repository;

    public GoogleKeepImport(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GetString(Stream stream)
    {
        List<GoogleKeepNote> googleKeepNotes = await GetGoogleKeepNotes(stream);

        string text = string.Empty;

        foreach (GoogleKeepNote googleKeepNote in googleKeepNotes.OrderByDescending(gkn => gkn.CreatedTimestampUsec))
        {
            if (!string.IsNullOrEmpty(googleKeepNote.Title))
            {
                string title = $"# {googleKeepNote.Title}";
                text += string.IsNullOrEmpty(text) ? title : "\n" + "\n" + title;
            }
            else if (!string.IsNullOrEmpty(text))
            {
                text += "\n";
            }

            if (!string.IsNullOrEmpty(googleKeepNote.TextContent))
            {
                text += string.IsNullOrEmpty(text) ? googleKeepNote.TextContent : "\n" + googleKeepNote.TextContent;
            }
        }

        return text;
    }

    public async Task ImportData(Stream stream)
    {
        List<GoogleKeepNote> googleKeepNotes = await GetGoogleKeepNotes(stream);

        CategoryModel category = _repository.Category;

        foreach (GoogleKeepNote googleKeepNote in googleKeepNotes.OrderByDescending(gkn => gkn.CreatedTimestampUsec))
        {
            GoalModel goal = category.CreateGoal(_repository.NextGoalId, googleKeepNote.Title);
            goal.Details = googleKeepNote.TextContent;

            await _repository.AddGoal(goal);

            TaskModel? task = null;

            foreach (string line in goal.Details.Split('\n'))
            {
                if (task is not null && line.StartsWith("- "))
                {
                    bool add = task.AddDetail(line);

                    if (add)
                        task.DetailsText += string.IsNullOrEmpty(task.DetailsText) ? line : Environment.NewLine + line;

                    await _repository.UpdateTask(task.Id);
                }
                else
                {
                    if (!goal.TaskList.Any())
                    {
                        goal.CreateTaskFromEachLine = true;
                        await _repository.UpdateGoal(goal.Id);
                    }

                    task = goal.CreateTask(_repository.NextTaskId, line);

                    await _repository.AddTask(task);
                }
            }
        }
    }

    private static async Task<List<GoogleKeepNote>> GetGoogleKeepNotes(Stream stream)
    {
        MemoryStream memoryStream = new();

        await stream.CopyToAsync(memoryStream);

        ZipArchive archive = new(memoryStream);

        List<GoogleKeepNote> googleKeepNotes = new();

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                await using Stream jsonStream = entry.Open();

                using StreamReader streamReader = new(jsonStream);

                string jsonText = await streamReader.ReadToEndAsync();

                GoogleKeepNote? googleKeepNote = JsonSerializer.Deserialize<GoogleKeepNote>(jsonText);

                if (googleKeepNote is not null)
                {
                    googleKeepNotes.Add(googleKeepNote);
                }
            }
        }

        return googleKeepNotes;
    }
}
