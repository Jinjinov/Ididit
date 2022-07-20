using Ididit.App;
using Ididit.Data;
using Ididit.Data.Models;
using Ididit.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ididit.UI.Pages;

public partial class Options
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [Parameter]
    public CategoryModel? SelectedCategory { get; set; } = null!;

    [Parameter]
    public IList<string> Themes { get; set; } = null!;

    [Parameter]
    public Blazorise.Size Size { get; set; }

    [Parameter]
    public string Theme { get; set; } = null!;

    [Parameter]
    public EventCallback<Blazorise.Size> SizeChanged { get; set; }

    [Parameter]
    public EventCallback<string> ThemeChanged { get; set; }

    async Task OnSizeChanged(Blazorise.Size size)
    {
        Size = size;
        await SizeChanged.InvokeAsync(Size);
    }

    async Task OnThemeChanged(string theme)
    {
        Theme = theme;
        await ThemeChanged.InvokeAsync(Theme);
    }

    [Inject]
    DirectoryBackup DirectoryBackup { get; set; } = null!;

    [Inject]
    JsonBackup JsonBackup { get; set; } = null!;

    [Inject]
    YamlBackup YamlBackup { get; set; } = null!;

    [Inject]
    TsvBackup TsvBackup { get; set; } = null!;

    [Inject]
    MarkdownBackup MarkdownBackup { get; set; } = null!;

    [Inject]
    JsInterop JsInterop { get; set; } = null!;

    async Task Import(InputFileChangeEventArgs e)
    {
        Stream stream = e.File.OpenReadStream(maxAllowedSize: 5242880);

        if (e.File.Name.EndsWith(".json"))
        {
            DataModel data = await JsonBackup.ImportData(stream);

            await Repository.AddData(data);
        }

        if (e.File.Name.EndsWith(".yaml"))
        {
            DataModel data = await YamlBackup.ImportData(stream);

            await Repository.AddData(data);
        }

        if (e.File.Name.EndsWith(".tsv"))
        {
            await TsvBackup.ImportData(stream);

            //await _repository.AddData(data);
        }
    }

    async Task ExportJson()
    {
        await JsonBackup.ExportData(Repository);
    }

    async Task ExportYaml()
    {
        await YamlBackup.ExportData(Repository);
    }

    async Task ExportTsv()
    {
        await TsvBackup.ExportData(Repository);
    }

    async Task ImportMarkdown(InputFileChangeEventArgs e)
    {
        if (SelectedCategory != null)
        {
            IEnumerable<IBrowserFile> browserFiles = e.GetMultipleFiles(e.FileCount).Where(browserFile => browserFile.Name.EndsWith(".md"));

            foreach (IBrowserFile browserFile in browserFiles)
            {
                string name = Path.GetFileNameWithoutExtension(browserFile.Name);

                Stream stream = browserFile.OpenReadStream();

                await MarkdownBackup.ImportData(SelectedCategory, stream, name);
            }
        }
    }

    async Task ExportMarkdown()
    {
        await MarkdownBackup.ExportData(Repository);
    }

    async Task ImportDirectory()
    {
        NodeContent? directory = await JsInterop.ReadDirectoryFiles();

        if (directory != null)
            await DirectoryBackup.ImportData(directory);
    }

    async Task ExportDirectory()
    {
        NodeContent[] nodes = DirectoryBackup.ExportData(Repository);

        await JsInterop.WriteDirectoryFiles(nodes);
    }
}
