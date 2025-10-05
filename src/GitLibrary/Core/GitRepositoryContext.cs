using System.IO.Abstractions;
using GitLibrary.Commands.Processes.Interfaces;

namespace GitLibrary.Core;

internal sealed class GitRepositoryContext(IDirectoryInfo directory, IProcessRunner runner)
{
    public IDirectoryInfo Directory { get; } = directory ?? throw new ArgumentNullException(nameof(directory));
    public IProcessRunner Runner { get; } = runner ?? throw new ArgumentNullException(nameof(runner));
}