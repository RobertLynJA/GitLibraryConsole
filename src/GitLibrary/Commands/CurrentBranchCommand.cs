using GitLibrary.Commands.Data;
using GitLibrary.Commands.Interfaces;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary.Commands;

internal class CurrentBranchCommand(IProcessRunner processRunner) : IGitCommand
{
    private readonly IProcessRunner _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    public string Name => "current-branch";
    public string Description => "Gets the current branch name.";

    public async Task<GitCommandResult> ExecuteAsync(GitCommandContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        
        var result = await _processRunner.RunAsync(
            "git",
            ["rev-parse", "--abbrev-ref", "HEAD"],
            context.DirectoryInfo.FullName,
            context.Environment,
            cancellationToken);
        return new GitCommandResult(result.ExitCode, result.StandardOutput.Trim(), result.StandardError);
    }
}