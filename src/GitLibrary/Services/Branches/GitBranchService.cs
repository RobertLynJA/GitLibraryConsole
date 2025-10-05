using GitLibrary.Commands;
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
                var isCurrent = rawLine.StartsWith('*');
                var line = rawLine.Trim().TrimStart('*').Trim();
                var isRemote = line.StartsWith("remotes/");
                var shortName = isRemote ? line.Substring("remotes/".Length) : line;
                var fullName = isRemote ? line : $"refs/heads/{shortName}";
                return new Branch(shortName, fullName, isCurrent, isRemote);
            })
            .ToList()
            .AsReadOnly();

        return branches;
    }
}