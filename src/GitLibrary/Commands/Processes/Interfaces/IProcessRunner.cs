using GitLibrary.Commands.Data;

namespace GitLibrary.Commands.Processes.Interfaces;

internal interface IProcessRunner
{
    Task<ProcessResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string>? environment,
        CancellationToken cancellationToken = default);
}
