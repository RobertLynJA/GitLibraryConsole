using System;
using System.IO.Abstractions;
using NSubstitute;
using Xunit;

namespace GitLibrary.Test;

public class GitTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenDirectoryIsNull()
    {
        //Arrange
        IDirectoryInfo directory = null!;
        
        //Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Git(directory));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNotAGitRepository()
    {
        //Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => new Git(dir));
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