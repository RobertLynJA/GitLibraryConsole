namespace GitLibrary.Commands.Data;

public sealed record GitCommandContext(
    System.IO.Abstractions.IDirectoryInfo DirectoryInfo,
    IReadOnlyDictionary<string, string>? Environment = null,
    IReadOnlyList<string>? AdditionalArguments = null);