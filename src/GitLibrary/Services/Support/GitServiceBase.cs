using GitLibrary.Commands.Data;
using GitLibrary.Commands.Interfaces;
using GitLibrary.Core;

namespace GitLibrary.Services.Support;

internal abstract class GitServiceBase
{
    protected GitRepositoryContext Ctx { get; }
    protected GitServiceBase(GitRepositoryContext ctx) => Ctx = ctx;

    protected async Task<GitCommandResult> RunAsync(IGitCommand command, CancellationToken token)
    {
        var gitContext = new GitCommandContext(Ctx.Directory);
        return await command.ExecuteAsync(gitContext, token);
    }

    protected static void EnsureSuccess(GitCommandResult result, string errorPrefix)
    {
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"{errorPrefix}: {result.StandardError}");
    }
}