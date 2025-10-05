using GitLibrary.Core;
using GitLibrary.Commands.Processes.Interfaces;
using System.IO.Abstractions;
using NSubstitute;

namespace GitLibrary.Test.Core;

public class GitRepositoryContextTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenDirectoryIsNull()
    {
        // Arrange
        var runner = Substitute.For<IProcessRunner>();
        
        // Act
        var ex = Record.Exception(() => new GitRepositoryContext(null!, runner));
        
        // Assert
        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal("directory", ((ArgumentNullException)ex).ParamName);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRunnerIsNull()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        
        // Act
        var ex = Record.Exception(() => new GitRepositoryContext(dir, null!));
        
        // Assert
        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal("runner", ((ArgumentNullException)ex).ParamName);
    }

    [Fact]
    public void Constructor_SetsProperties_WhenArgumentsValid()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var runner = Substitute.For<IProcessRunner>();

        // Act
        var ctx = new GitRepositoryContext(dir, runner);

        // Assert
        Assert.Same(dir, ctx.Directory);
        Assert.Same(runner, ctx.Runner);
    }
}