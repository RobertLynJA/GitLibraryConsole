using GitLibrary.Commands;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary;

public class Git
{
    private readonly System.IO.Abstractions.IDirectoryInfo _directoryInfo;
    
    // For testing purposes, allow injection of a custom process runner.
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
    
    public async Task<IReadOnlyList<Data.Branch>> GetAllBranchesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var allBranchesCommand = new AllBranchesCommand(ProcessRunner);
        var context = new GitCommandContext(_directoryInfo);
        var result = await allBranchesCommand.ExecuteAsync(context, cancellationToken);
        if (!result.Success)
            throw new InvalidOperationException($"Failed to get branches: {result.StandardError}");
        
        var branches = result.StandardOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(rawLine =>
            {
                var isCurrent = rawLine.StartsWith('*');
                var line = rawLine.Trim().TrimStart('*').Trim();

                var isRemote = line.StartsWith("remotes/");
                // Short (display) name
                var shortName = isRemote
                    ? line.Substring("remotes/".Length)          // e.g. remotes/origin/main -> origin/main
                    : line;

                // Full ref name
                var fullName = isRemote
                    ? line                                       // keep the original remote path
                    : $"refs/heads/{shortName}";

                return new Data.Branch(shortName, fullName, isCurrent, isRemote);
            })
            .ToList()
            .AsReadOnly();
        
        return branches;
        
    }
}