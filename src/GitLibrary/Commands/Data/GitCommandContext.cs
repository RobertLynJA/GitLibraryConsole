namespace GitLibrary.Commands.Data;

internal sealed record GitCommandContext(
    System.IO.Abstractions.IDirectoryInfo DirectoryInfo,
    IReadOnlyDictionary<string, string>? Environment = null,
    IReadOnlyList<string>? AdditionalArguments = null);