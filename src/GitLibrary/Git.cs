using System.IO.Abstractions;
using GitLibrary.Commands;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes;
using GitLibrary.Commands.Processes.Interfaces;
using GitLibrary.Core;
using GitLibrary.Services;
using GitLibrary.Services.Branches;
using GitLibrary.Services.Interfaces;

namespace GitLibrary;

public class Git
{
    // For testing purposes, allow injection of a custom process runner.
    internal IProcessRunner ProcessRunner = new ProcessRunner();
    
    public IGitBranchService Branches { get; }
    
    public Git(System.IO.Abstractions.IDirectoryInfo directoryInfo)
    {
        ArgumentNullException.ThrowIfNull(directoryInfo);

        if (!Utils.GitRepositoryUtils.IsGitRepository(directoryInfo))
        {
            throw new ArgumentException("The provided path is not a Git repository.");
        }
        
        var ctx = new GitRepositoryContext(directoryInfo, ProcessRunner);

        Branches = new GitBranchService(ctx);
        // Future:
        // Remotes = new GitRemoteService(ctx);
        // Commits = new GitCommitService(ctx);
    }
}