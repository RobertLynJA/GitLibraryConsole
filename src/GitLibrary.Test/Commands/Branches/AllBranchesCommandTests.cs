namespace GitLibrary.Test.Commands.Branches;

using GitLibrary.Commands.Branches;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes.Interfaces;
using NSubstitute;
using Xunit;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

public class AllBranchesCommandTests
{
    [Fact]
    public async Task ExecuteAsync_PassesExpectedArgumentsAndWorkingDirectory()
    {
        var runner = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");
        var context = new GitCommandContext(dir);
        IReadOnlyList<string>? capturedArgs = null;

        runner.RunAsync(
            "git",
            Arg.Do<IReadOnlyList<string>>(a => capturedArgs = a),
            dir.FullName,
            context.Environment,
            Arg.Any<CancellationToken>())
        .Returns(new ProcessResult(0, string.Empty, string.Empty));

        var cmd = new AllBranchesCommand(runner);
        await cmd.ExecuteAsync(context);

        Assert.NotNull(capturedArgs);
        Assert.Contains("for-each-ref", capturedArgs!);
        Assert.Contains("--exclude=refs/remotes/*/HEAD", capturedArgs!);
        Assert.Contains("refs/heads/", capturedArgs!);
        Assert.Contains("refs/remotes/", capturedArgs!);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsResultFromProcessRunner()
    {
        var runner = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");
        var context = new GitCommandContext(dir);
        var output = "abc123 refs/heads/main main *";
        runner.RunAsync(
            "git",
            Arg.Any<IReadOnlyList<string>>(),
            dir.FullName,
            context.Environment,
            Arg.Any<CancellationToken>())
        .Returns(new ProcessResult(0, output, string.Empty));

        var cmd = new AllBranchesCommand(runner);
        var result = await cmd.ExecuteAsync(context);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(output, result.StandardOutput);
        Assert.Equal(string.Empty, result.StandardError);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenContextNull()
    {
        var runner = Substitute.For<IProcessRunner>();
        var cmd = new AllBranchesCommand(runner);
        await Assert.ThrowsAsync<ArgumentNullException>(() => cmd.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsOperationCanceled_WhenTokenCancelled()
    {
        var runner = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");
        var context = new GitCommandContext(dir);
        var cmd = new AllBranchesCommand(runner);
        var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(() => cmd.ExecuteAsync(context, cts.Token));
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotAlterOutput()
    {
        var runner = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");
        var context = new GitCommandContext(dir);
        var output = "111aaa refs/heads/main main *\n222bbb refs/remotes/origin/develop develop";
        runner.RunAsync(
            "git",
            Arg.Any<IReadOnlyList<string>>(),
            dir.FullName,
            context.Environment,
            Arg.Any<CancellationToken>())
        .Returns(new ProcessResult(0, output, string.Empty));

        var cmd = new AllBranchesCommand(runner);
        var result = await cmd.ExecuteAsync(context);

        Assert.Equal(output, result.StandardOutput);
    }
}