using System.IO.Abstractions;
using GitLibrary;

namespace GitLibraryConsole;

class Program
{
    async static Task Main(string[] args)
    {
        Git git = new((DirectoryInfoBase)new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.Parent);
        
        var branches2 = await git.Branches.GetAllBranchesAsync();
        Console.WriteLine("All branches:");
        foreach (var branch in branches2)
        {
            Console.WriteLine(branch);
        }
            
    }
}