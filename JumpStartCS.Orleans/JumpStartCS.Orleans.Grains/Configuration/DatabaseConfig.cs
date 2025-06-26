namespace Roshambofu.Configuration;

/// <summary>
/// PostgreSQL database config settings
/// </summary>
public class DatabaseConfig
{
	/// <summary>
	/// Just the host DNS name part of the URI
	/// </summary>
	public string Host { get; set; }
 
	/// <summary>
	/// The database name
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// The user's plaintext password
	/// </summary>
	public string Password { get; set; }

	/// <summary>
	/// The username
	/// </summary>
	public string Username { get; set; }

	public DatabaseConfig()
	{
		Host = "";
		Name = "";
		Password = "";
		Username = "";
	}
}
