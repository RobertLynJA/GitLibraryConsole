namespace GitLibrary.Commands.Data;

internal sealed record ProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError
);