namespace GitLibrary;

public class Git
{
    private readonly System.IO.Abstractions.IDirectoryInfo _directoryInfo;
    
    public Git(System.IO.Abstractions.IDirectoryInfo directoryInfo)
    {
        _directoryInfo = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));

        if (!IsGitRepository(_directoryInfo))
        {
            throw new ArgumentException("The provided path is not a Git repository.");
        }
    }
    
    public static bool IsGitRepository(System.IO.Abstractions.IDirectoryInfo directoryInfo)
    {
        return directoryInfo.Exists && directoryInfo.GetDirectories(".git").Length > 0;
    }
}