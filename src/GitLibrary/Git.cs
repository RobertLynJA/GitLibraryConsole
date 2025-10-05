using GitLibrary.Commands;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary;

public class Git
{
    private readonly System.IO.Abstractions.IDirectoryInfo _directoryInfo;
    internal IProcessRunner ProcessRunner = new ProcessRunner();
    
    public Git(System.IO.Abstractions.IDirectoryInfo directoryInfo)
    {
        _directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));

        if (!Utils.GitRepositoryUtils.IsGitRepository(_directoryInfo))
        {
            throw new ArgumentException("The provided path is not a Git repository.");
        }
    }
    
    public async Task<string> GetCurrentBranchNameAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var currentBranchCommand = new CurrentBranchCommand(ProcessRunner);
        var context = new GitCommandContext(_directoryInfo);
        var result = await currentBranchCommand.ExecuteAsync(context, cancellationToken);
        if (!result.Success)
            throw new InvalidOperationException($"Failed to get current branch: {result.StandardError}");
        return result.StandardOutput.Trim();
    }
}