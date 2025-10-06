namespace GitLibrary.Services.Interfaces;

public interface IGitBranchService
{
    Task<string> GetCurrentBranchNameAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Data.Branch>> GetAllBranchesAsync(CancellationToken cancellationToken = default);
}