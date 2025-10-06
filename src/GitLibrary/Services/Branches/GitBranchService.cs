using GitLibrary.Commands;
using GitLibrary.Commands.Branches;
using GitLibrary.Data;
using GitLibrary.Services.Support;
using GitLibrary.Core;

namespace GitLibrary.Services.Branches;

internal sealed class GitBranchService(GitRepositoryContext ctx) : GitServiceBase(ctx), Interfaces.IGitBranchService
{
    public async Task<string> GetCurrentBranchNameAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var cmd = new CurrentBranchCommand(Ctx.Runner);
        var result = await RunAsync(cmd, cancellationToken);
        EnsureSuccess(result, "Failed to get current branch");
        return result.StandardOutput.Trim();
    }
    
    public async Task<IReadOnlyList<Branch>> GetAllBranchesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var cmd = new AllBranchesCommand(Ctx.Runner);
        var result = await RunAsync(cmd, cancellationToken);
        EnsureSuccess(result, "Failed to get branches");

        var branches = result.StandardOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(rawLine =>
            {
                var line = rawLine.Trim().Trim('\'').Trim();
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3)
                    throw new InvalidOperationException($"Unexpected branch line format: '{rawLine}'");
                
                var isCurrent = parts.Length > 3 && parts[3] == "*";
                var fullName = parts[1];
                var shortName = parts[2];
                var hashCode = parts[0];
                var isRemote = fullName.StartsWith("refs/remotes/");
                return new Branch(fullName, shortName, isCurrent, isRemote, hashCode);
            })
            .ToList()
            .AsReadOnly();
        
        return branches;
    }
}