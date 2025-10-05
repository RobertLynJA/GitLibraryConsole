using System.IO.Abstractions;
using NSubstitute;

namespace GitLibrary.Test;

public class GitTests
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
}