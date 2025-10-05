using System.IO.Abstractions;
using GitLibrary;

namespace GitLibraryConsole;

class Program
{
    async static Task Main(string[] args)
    {
        Git git = new((DirectoryInfoBase)new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.Parent);

        var currentBranch = await git.Branches.GetCurrentBranchNameAsync();
        Console.WriteLine($"Current branch: {currentBranch}");
        
        var branches = await git.Branches.GetAllBranchesAsync();
        Console.WriteLine("All branches:");
        foreach (var branch in branches)
        {
            Console.WriteLine(branch);
        }
            
    }
}