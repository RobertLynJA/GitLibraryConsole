using GitLibrary.Commands.Data;
using GitLibrary.Commands.Interfaces;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary.Commands.Branches;

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
                [
                    "for-each-ref", 
                    "--sort=-committerdate", 
                    "--exclude=refs/remotes/*/HEAD", 
                    "--format='%(objectname) %(refname) %(refname:short) %(if)%(HEAD)%(then)*%(end)'", 
                    "refs/heads/", "refs/remotes/"
                ],
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