using System.Collections.Generic;
using System.Linq;

namespace Ididit.Backup;

internal class ImportExport : IImportExport
{
    public IReadOnlyDictionary<string, IFileImport> FileImportByExtension => _fileImportByExtension;
    public IReadOnlyDictionary<DataFormat, IDataExport> DataExportByFormat => _dataExportByFormat;

    private readonly Dictionary<string, IFileImport> _fileImportByExtension;
    private readonly Dictionary<DataFormat, IDataExport> _dataExportByFormat;

    public ImportExport(IEnumerable<IFileImport> fileImports, IEnumerable<IDataExport> dataExports)
    {
        _fileImportByExtension = fileImports.ToDictionary(fi => fi.FileExtension, fi => fi);
        _dataExportByFormat = dataExports.ToDictionary(de => de.DataFormat, de => de);
    }
}
