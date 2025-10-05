using GitLibrary.Commands.Data;
using GitLibrary.Commands.Interfaces;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary.Commands;

internal class AllBranchesCommand(IProcessRunner processRunner) : IGitCommand
{
    private readonly IProcessRunner _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    public string Name => "all-branches";
    public string Description => "Gets a list of all branches.";
    
    public Task<GitCommandResult> ExecuteAsync(GitCommandContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        
        return _processRunner.RunAsync(
            "git",
            ["branch", "--list", "--all", "--no-color"],
            context.DirectoryInfo.FullName,
            context.Environment,
            cancellationToken)
            .ContinueWith(task =>
            {
                var result = task.Result;
                return new GitCommandResult(result.ExitCode, result.StandardOutput, result.StandardError);
            }, cancellationToken);
    }
}