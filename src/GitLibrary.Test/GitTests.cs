using System;
using System.IO.Abstractions;
using NSubstitute;
using Xunit;

namespace GitLibrary.Test;

public class GitTests
{
    [Fact]
    public void IsGitRepository_ReturnsTrue_WhenGitDirectoryExists()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);

        Assert.True(Git.IsGitRepository(dir));
    }

    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenDirectoryDoesNotExist()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(false);

        Assert.False(Git.IsGitRepository(dir));
    }
    
    [Fact]
    public void IsGitRepository_ReturnsFalse_WhenGitDirectoryDoesNotExist()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        Assert.False(Git.IsGitRepository(dir));
    }
}