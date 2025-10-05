using GitLibrary.Commands.Data;

namespace GitLibrary.Test.Commands.Data;

public class GitCommandResultTests
{
    [Fact]
    public void Success_ShouldBeTrue_WhenExitCodeIsZero()
    {
        // Arrange
        var result = new GitCommandResult(0, "output", "error");

        // Act
        var success = result.Success;

        // Assert
        Assert.True(success);
    }

    [Fact]
    public void Success_ShouldBeFalse_WhenExitCodeIsNonZero()
    {
        // Arrange
        var result = new GitCommandResult(1, "output", "error");

        // Act
        var success = result.Success;

        // Assert
        Assert.False(success);
    }
}