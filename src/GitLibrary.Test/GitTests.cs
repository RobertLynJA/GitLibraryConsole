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
        Assert.Throws<ArgumentNullException>(() => new Git(null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNotAGitRepository()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([]);

        Assert.Throws<ArgumentException>(() => new Git(dir));
    }

    [Fact]
    public void Constructor_Succeeds_WhenGitRepositoryExists()
    {
        var dir = Substitute.For<IDirectoryInfo>();
        var gitDir = Substitute.For<IDirectoryInfo>();
        dir.Exists.Returns(true);
        dir.GetDirectories(".git").Returns([gitDir]);

        var git = new Git(dir);
        Assert.NotNull(git);
    }
}