using System.Collections.Generic;

namespace Ididit.Backup;

internal interface IImportExport
{
    IReadOnlyDictionary<string, IFileImport> FileImportByExtension { get; }
    IReadOnlyDictionary<string, IFileToString> FileToStringByExtension { get; }
    IReadOnlyDictionary<DataFormat, IDataExport> DataExportByFormat { get; }
}
