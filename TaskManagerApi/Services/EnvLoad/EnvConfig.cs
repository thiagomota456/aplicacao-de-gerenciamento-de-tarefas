namespace TaskManagerApi.Services.EnvLoad;

public static class EnvConfig
{
    public static void Load(string? path = null)
    {
        path ??= Path.Combine(Directory.GetCurrentDirectory(), ".env");

        if (!File.Exists(path)) 
            return;

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) 
                continue;

            var idx = line.IndexOf('=');
            if (idx <= 0) continue;

            var key = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim();

            // tira aspas se vierem
            if ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'")))
            {
                value = value[1..^1];
            }
            
            key = key.Replace(":", "__");

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
