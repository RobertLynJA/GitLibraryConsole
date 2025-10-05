using System.IO.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit.Abstractions;

namespace GitLibrary.Test;

public class GitTests(ITestOutputHelper output)
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenDirectoryIsNull()
    {
        // Arrange
        IDirectoryInfo directory = null!;
        
        // Act
        var ex = Record.Exception(() => new Git(directory));
        
        // Assert
        Assert.IsType<ArgumentNullException>(ex);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNotAGitRepository()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        // Act
        var ex = Record.Exception(() => new Git(dir));

        // Assert
        Assert.IsType<ArgumentException>(ex);
    }

    [Fact]
    public void Constructor_Succeeds_WhenGitRepositoryExists()
    {
        //Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);

        //Act
        var git = new Git(dir);
        
        //Assert
        Assert.NotNull(git);
    }
    
    [Fact]
    public async Task GetCurrentBranchNameAsync_ReturnsCheckedOutBranch()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);
        var processRunner = Substitute.For<GitLibrary.Commands.Processes.Interfaces.IProcessRunner>();
        processRunner.RunAsync(
            "git",
            Arg.Is<IReadOnlyList<string>>(a => a.SequenceEqual(new[] { "rev-parse", "--abbrev-ref", "HEAD" })),
            dir.FullName,
            Arg.Any<IReadOnlyDictionary<string, string>>(),
            Arg.Any<CancellationToken>())
        .Returns(new GitLibrary.Commands.Data.ProcessResult(0, "main", string.Empty));
        var git = new Git(dir);
        
        // Act
        var name = await git.GetCurrentBranchNameAsync();

        // Assert
        Assert.Equal("main", name);
    }

    [Fact]
    public async Task GetCurrentBranchNameAsync_ThrowsInvalidOperation_WhenGitFails()
    {
        //Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);
        var processRunner = Substitute.For<GitLibrary.Commands.Processes.Interfaces.IProcessRunner>();
        processRunner.RunAsync(
            "git",
            Arg.Is<IReadOnlyList<string>>(a => a.SequenceEqual(new[] { "rev-parse", "--abbrev-ref", "HEAD" })),
            dir.FullName,
            Arg.Any<IReadOnlyDictionary<string, string>>(),
            Arg.Any<CancellationToken>())
        .ThrowsAsync(new InvalidOperationException());
        var git = new Git(dir);
        
        //Act
        var name = await git.GetCurrentBranchNameAsync();

        // Assert
        Assert.Equal("main", name);
    }

    [Fact]
    public async Task GetCurrentBranchNameAsync_ThrowsOperationCanceled_WhenCancelled()
    {
        //Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);
        var processRunner = Substitute.For<GitLibrary.Commands.Processes.Interfaces.IProcessRunner>();
        processRunner.RunAsync(
                "git",
                Arg.Is<IReadOnlyList<string>>(a => a.SequenceEqual(new[] { "rev-parse", "--abbrev-ref", "HEAD" })),
                dir.FullName,
                Arg.Any<IReadOnlyDictionary<string, string>>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException());
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        var git = new Git(dir);
        
        //Act
        var ex = await Record.ExceptionAsync(() => git.GetCurrentBranchNameAsync(cancellationTokenSource.Token));
        
        // Assert
        Assert.NotNull(ex);
        output.WriteLine("Caught exception: {0}", ex.GetType().FullName);
        Assert.IsType<OperationCanceledException>(ex);
    }
}