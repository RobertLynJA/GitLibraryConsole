namespace GitLibrary.Data;

public record Branch(string Name, string ShortName, bool IsCurrent, bool IsRemote, string Hash);

