using System.IO.Abstractions;
using GitLibrary.Utils;
using NSubstitute;

namespace GitLibrary.Test.Utils;

public class GitRepositoryUtilsTests
{
    [Fact]
    public void IsGitRepository_ReturnsTrue_WhenGitDirectoryExists()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);

        Assert.True(GitRepositoryUtils.IsGitRepository(dir));
    }

    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenDirectoryDoesNotExist()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(false);

        Assert.False(GitRepositoryUtils.IsGitRepository(dir));
    }
    
    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenGitDirectoryDoesNotExist()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        Assert.False(GitRepositoryUtils.IsGitRepository(dir));
    }
}