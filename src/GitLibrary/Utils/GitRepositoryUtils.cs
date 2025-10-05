namespace GitLibrary.Utils;

public static class GitRepositoryUtils
{
    public static bool IsGitRepository(System.IO.Abstractions.IDirectoryInfo directoryInfo)
    {
        return directoryInfo.Exists && directoryInfo.GetDirectories(".git").Length > 0;
    }
}