namespace GitLibrary.Commands.Data;

internal sealed record GitCommandResult(
    int ExitCode,
    string StandardOutput,
    string StandardError)
{
    public bool Success => ExitCode == 0;
}