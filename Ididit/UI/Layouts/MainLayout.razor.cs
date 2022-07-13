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

namespace Ididit.UI.Layouts;

public partial class MainLayout
{
    public Blazorise.Size Size => Repository.Settings.Size;

    readonly SortedList<string, string> _bootswatchThemes = new()
    {
        { "cerulean", "sha256-RW/ojwB9sS6DzIfFEaDiahxVPN/MupUlrbKrKVkgA9M=" },
        { "cosmo", "sha256-/H81OfRt8XcW5i6dfB86YU8k0XTUbj2ezDqejIrXWs4=" },
        { "cyborg", "sha256-3FdPvyIyBF62iTAVS30syY0eik+vm4G1fiCPErB7IbQ=" },
        { "darkly", "sha256-0tZXz4Wl0mk302PbATyRKjsHT1tI5FwnK5gY8DB5SeY=" },
        { "flatly", "sha256-K0Q0THV+T7huqMhZI87+P0A/uy+tbcyC751i8ScYytw=" },
        { "journal", "sha256-b9ANi+bPfqDaEOG9y/CZ2+cs5WlO6MdZ85ZEDwp99aw=" },
        { "litera", "sha256-YwRe9tp/LgR/N/G0wb0GXHfupruWc0Pf6Juel/cY+G8=" },
        { "lumen", "sha256-jlJkgYQWWRqOYNoAJrtSPAgJ8XzAB5oW8G8/ovdz/x0=" },
        { "lux", "sha256-v2HsAdJ0L9YjVjheYom4BSraR+nwJDgYj6wI73QIedI=" },
        { "materia", "sha256-MEhiSVHcxsJGoCn/OncBKfjYzl8Z+gHOA/TJ0s4oTP8=" },
        { "minty", "sha256-dH/ZJzNkhrTqLl30dAJ0pIDbuNWlF2Fht9I2Q8jhrcs=" },
        { "pulse", "sha256-1vQy0iTuGRchdjqh8TwEkCeFn81PyVt9KofS69uQxKs=" },
        { "sandstone", "sha256-Wj8Vq1ER7uigVDcLe8Kfwxu4ieW867kr9HQihUUfnQA=" },
        { "simplex", "sha256-Xsqt9TSw+DbYFdZSxbG/pMRF/JLdDeG6xFFRU3JwWxs=" },
        { "sketchy", "sha256-tT30p4dCoDHNDmxveHbPJQSFVn/7a7pDBqplXFNWff4=" },
        { "slate", "sha256-qdwWE5kCHBJrfGXA4BRFz6ixKw9PCpaYzJWA9260DzY=" },
        { "solar", "sha256-eeFZTjMs+Fz5lW8l5wrcJztSyJmQNRFu9FtwqSDOcaY=" },
        { "spacelab", "sha256-VdSljf9GE8Znxpt5oe70r3Yffh2TuRl1m+vMztZP2/4=" },
        { "superhero", "sha256-ADi3Xne8BjmmGG8K4lQWmU7SegWM4B2xWeapboEdA18=" },
        { "united", "sha256-7gdx77MsYTQV0shn+8C0CP7lc6xQRexoNubpaigHL6c=" },
        { "yeti", "sha256-hqMfmPlwrJpRN1EanXFcujx+vGhMfBbeKVyT09X3mIw=" }
    };

    [Inject]
    IRepository Repository { get; set; } = null!;

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
        // TODO: use the real selectedCategory

        CategoryModel? selectedCategory = Repository.CategoryList.FirstOrDefault();

        if (selectedCategory != null)
        {
            IEnumerable<IBrowserFile> browserFiles = e.GetMultipleFiles(e.FileCount).Where(browserFile => browserFile.Name.EndsWith(".md"));

            foreach (IBrowserFile browserFile in browserFiles)
            {
                string name = Path.GetFileNameWithoutExtension(browserFile.Name);

                Stream stream = browserFile.OpenReadStream();

                await MarkdownBackup.ImportData(selectedCategory, stream, name);
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
