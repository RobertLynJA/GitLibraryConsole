using GitLibrary.Commands;
using GitLibrary.Commands.Data;
using GitLibrary.Commands.Processes.Interfaces;
using NSubstitute;
using System.IO.Abstractions;
using Xunit.Abstractions;

namespace GitLibrary.Test.Commands;

public class AllBranchesCommandTests(ITestOutputHelper output)
{
    [Fact]
    public async Task ExecuteAsync_ReturnsBranchNames_WhenGitCommandSucceeds()
    {
        // Arrange
        var processRunnerMock = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");

        var context = new GitCommandContext(dir, new Dictionary<string,string>());
        processRunnerMock
            .RunAsync(
                "git",
                Arg.Is<IReadOnlyList<string>>(args => args.SequenceEqual(new[] { "branch", "--list", "--all", "--no-color" })),
                context.DirectoryInfo.FullName,
                context.Environment,
                Arg.Any<CancellationToken>())
            .Returns(new ProcessResult(0, "main\ndevelop\nfeature-xyz", string.Empty));
        var command = new AllBranchesCommand(processRunnerMock);

        // Act
        var result = await command.ExecuteAsync(context);

        // Assert
        Assert.Equal(0, result.ExitCode);
        Assert.Equal("main\ndevelop\nfeature-xyz", result.StandardOutput);
        Assert.Equal(string.Empty, result.StandardError);
    }
    
    [Fact]
    public async Task ExecuteAsync_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Arrange
        var processRunnerMock = Substitute.For<IProcessRunner>();
        var command = new CurrentBranchCommand(processRunnerMock);

        // Act
        var ex = await Record.ExceptionAsync(() => command.ExecuteAsync(null!));
        
        // Assert
        Assert.NotNull(ex);
        output.WriteLine("Caught exception: {0}", ex.GetType().FullName);
        Assert.IsType<ArgumentNullException>(ex);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsError_WhenGitCommandFails()
    {
        // Arrange
        var processRunnerMock = Substitute.For<IProcessRunner>();
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");
        
        var context = new GitCommandContext(dir, new Dictionary<string,string>());
        processRunnerMock
            .RunAsync(
                "git",
                Arg.Is<IReadOnlyList<string>>(args => args.SequenceEqual(new[] { "branch", "--list", "--all", "--no-color" })),
                context.DirectoryInfo.FullName,
                context.Environment,
                Arg.Any<CancellationToken>())
            .Returns(new ProcessResult(1, string.Empty, "fatal: not a git repository"));
        var command = new AllBranchesCommand(processRunnerMock);

        // Act
        var result = await command.ExecuteAsync(context);

        // Assert
        Assert.Equal(1, result.ExitCode);
        Assert.Equal(string.Empty, result.StandardOutput);
        Assert.Equal("fatal: not a git repository", result.StandardError);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesCancellation_WhenTokenIsCancelled()
    {
        // Arrange
        var processRunnerMock = Substitute.For<IProcessRunner>();
        
        var dir = Substitute.For<IDirectoryInfo>();
        dir.FullName.Returns("/repo");

        var context = new GitCommandContext(dir, new Dictionary<string,string>());
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var command = new AllBranchesCommand(processRunnerMock);

        // Act
        var ex = await Record.ExceptionAsync(() => command.ExecuteAsync(context, cancellationTokenSource.Token));

        // Assert    
        Assert.NotNull(ex);
        output.WriteLine("Caught exception: {0}", ex.GetType().FullName);
        Assert.IsType<OperationCanceledException>(ex);
    }    
}