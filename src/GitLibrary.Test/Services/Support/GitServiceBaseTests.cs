using NSubstitute;
using GitLibrary.Services.Support;
using GitLibrary.Core;
using GitLibrary.Commands.Interfaces;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes.Interfaces;
using System.IO.Abstractions;

namespace GitLibrary.Test.Services.Support;

public class GitServiceBaseTests
{   
    private sealed class TestGitService : GitServiceBase
    {
        public TestGitService(GitRepositoryContext ctx) : base(ctx) { }
        public Task<GitCommandResult> RunForTestAsync(IGitCommand command, CancellationToken token = default) => RunAsync(command, token);
        public void EnsureSuccessForTest(GitCommandResult result, string prefix) => EnsureSuccess(result, prefix);
    }

    [Fact]
    public async Task RunAsync_ReturnsResultFromCommand()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var command = Substitute.For<IGitCommand>();
        var expected = new GitCommandResult(0, "out", "err");
        command.ExecuteAsync(Arg.Any<GitCommandContext>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expected));

        var result = await service.RunForTestAsync(command);

        Assert.Same(expected, result);
    }

    [Fact]
    public void EnsureSuccess_DoesNotThrow_WhenExitCodeZero()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var result = new GitCommandResult(0, "out", "");

        var ex = Record.Exception(() => service.EnsureSuccessForTest(result, "prefix"));

        Assert.Null(ex);
    }

    [Fact]
    public void EnsureSuccess_ThrowsInvalidOperation_WhenExitCodeNonZero()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var result = new GitCommandResult(1, "", "failure");

        var ex = Assert.Throws<InvalidOperationException>(() => service.EnsureSuccessForTest(result, "Failed"));
        Assert.Contains("Failed", ex.Message);
        Assert.Contains("failure", ex.Message);
    }

    [Fact]
    public async Task RunAsync_PassesRepositoryDirectoryToCommand()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var command = Substitute.For<IGitCommand>();
        command.ExecuteAsync(Arg.Any<GitCommandContext>(), Arg.Any<CancellationToken>())
            .Returns(ci => Task.FromResult(new GitCommandResult(0, "", "")));

        await service.RunForTestAsync(command);

        await command.Received(1).ExecuteAsync(Arg.Is<GitCommandContext>(g => ReferenceEquals(g.DirectoryInfo, dir)), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_ThrowsOperationCanceled_WhenTokenCancelledBeforeCall()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var command = Substitute.For<IGitCommand>();
        command.ExecuteAsync(Arg.Any<GitCommandContext>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var ct = ci.Arg<CancellationToken>();
                ct.ThrowIfCancellationRequested();
                return Task.FromResult(new GitCommandResult(0, "", ""));
            });

        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => service.RunForTestAsync(command, cts.Token));
    }

    [Fact]
    public async Task RunAsync_PropagatesCommandException()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();
        var ctx = new GitRepositoryContext(dir, runner);
        var service = new TestGitService(ctx);
        var command = Substitute.For<IGitCommand>();
        command.ExecuteAsync(Arg.Any<GitCommandContext>(), Arg.Any<CancellationToken>())
            .Returns<Task<GitCommandResult>>(x => throw new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RunForTestAsync(command));
        Assert.Equal("boom", ex.Message);
    }
}