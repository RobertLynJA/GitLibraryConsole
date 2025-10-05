namespace GitLibrary.Data;

public record Branch(string Name, string FullName, bool IsCurrent, bool IsRemote);

