using System.Diagnostics;
using System.Text;
using GitLibrary.Commands.Data;

namespace GitLibrary.Commands.Processes;

internal class ProcessRunner : Interfaces.IProcessRunner
{
    public async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, string workingDirectory, IReadOnlyDictionary<string, string>? environment,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(fileName);
        ArgumentNullException.ThrowIfNull(workingDirectory);
        ArgumentNullException.ThrowIfNull(arguments);
        
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        foreach (var arg in arguments)
            psi.ArgumentList.Add(arg);
        
        if (environment != null)
        {
            foreach (var kvp in environment)
                psi.Environment[kvp.Key] = kvp.Value;
        }

        using var process = new Process();
        process.StartInfo = psi;
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
        process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken);

        return new ProcessResult(
            process.ExitCode,
            outputBuilder.ToString(),
            errorBuilder.ToString()
        );
    }
}