using GitLibrary.Commands.Data;

namespace GitLibrary.Commands.Interfaces;

internal interface IGitCommand
{
    string Name { get; }
    string Description { get; }

    Task<GitCommandResult> ExecuteAsync(GitCommandContext context, CancellationToken cancellationToken = default);
}