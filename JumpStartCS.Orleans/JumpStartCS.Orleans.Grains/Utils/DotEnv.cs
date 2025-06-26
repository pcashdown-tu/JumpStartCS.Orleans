namespace Roshambofu.Utils;

/// <remarks>
/// <see href="https://dusted.codes/dotenv-in-dotnet" />
/// </remarks>
public static class DotEnv
{
	/// <summary>
	/// Loads and parses a .env file and sets environment variables in this
	/// process
	/// </summary>
	/// <param name="filePath"></param>
	public static bool Load(string? filePath = null)
	{
		if (string.IsNullOrWhiteSpace(filePath)) {
			// Search in current directory or executable's directory
			var dir = Directory.GetCurrentDirectory();
			filePath = Path.Combine(dir, ".env");
			if (!File.Exists(filePath)) {
				dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				filePath = Path.Combine(dir ?? "", ".env");
			}
			if (!File.Exists(filePath)) {
				Console.WriteLine($"Environment file {filePath} not found");
				return false;
			}
		} else if (!File.Exists(filePath)) {
			throw new FileNotFoundException(filePath);
		}

		foreach (var rawLine in File.ReadAllLines(filePath)) {
			var line = rawLine.TrimStart();
			if (string.IsNullOrEmpty(line) || line[0] == '#')
				continue;
			string? name = null;
			string? value = null;
			for (int i = 0; i < line.Length; ++i) {
				char c = line[i];
				if (!char.IsLetterOrDigit(c) && c != '_') {
					string rest = line[i..].TrimStart();
					if (rest[0] != '=')
						break;
					name = line[..i];
					value = rest.TrimStart(['=', ' ', '\t']);
				}
			}
			if (string.IsNullOrWhiteSpace(name))
				continue;
			Environment.SetEnvironmentVariable(name, value);
		}
		return true;
	}
}
