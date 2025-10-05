using System.IO.Abstractions;
using GitLibrary.Utils;
using NSubstitute;

namespace GitLibrary.Test.Utils;

public class GitRepositoryUtilsTests
{
    [Fact]
    public void IsGitRepository_ReturnsTrue_WhenGitDirectoryExists()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);

        // Act 
        var isGitRepository = GitRepositoryUtils.IsGitRepository(dir);
        
        // Assert
        Assert.True(isGitRepository);
    }

    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(false);

        // Act
        var isGitRepository = GitRepositoryUtils.IsGitRepository(dir);
        
        // Assert
        Assert.False(isGitRepository);
    }
    
    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenGitDirectoryDoesNotExist()
    {
        // Arrange
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        // Act
        var isGitRepository = GitRepositoryUtils.IsGitRepository(dir);
        
        // Assert
        Assert.False(isGitRepository);
    }
}