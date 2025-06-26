using System.Reflection;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Roshambofu.Configuration;

namespace Roshambofu.Utils;

public class DbUtils
{
	public static string GetConnectionString(IConfiguration? configOverride = null)
	{
		DatabaseConfig? dbConfig;
		if (configOverride != null) {
			dbConfig = configOverride.GetSection("DataBase").Get<DatabaseConfig>();
		} else {
			dbConfig = UtilsHelper.GetConfig<DatabaseConfig>("Database");
		}
		return $"Host={dbConfig?.Host};Database={dbConfig?.Name};Username={dbConfig?.Username};Password={dbConfig?.Password}";
	}

	public static string GetDbName()
	{
		var config = UtilsHelper.GetConfig<DatabaseConfig>("Database");
		return config.Name;
	}

	public static void SetupAdoNetGrainStorageTables()
	{
		using NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString());
		connection.Open();

		// See: https://learn.microsoft.com/en-us/dotnet/orleans/host/configuration-guide/adonet-configuration

		// From: https://github.com/dotnet/orleans/blob/main/src/AdoNet/Shared/PostgreSQL-Main.sql
		if (!TableExists(connection, "orleansquery")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Main.sql");
		}

		// From: https://github.com/dotnet/orleans/blob/main/src/AdoNet/Orleans.Clustering.AdoNet/PostgreSQL-Clustering.sql
		// And https://github.com/dotnet/orleans/blob/main/src/AdoNet/Orleans.Clustering.AdoNet/Migrations/PostgreSQL-Clustering-3.7.0.sql
		if (!TableExists(connection, "orleansmembershipversiontable")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Clustering.sql");
		}

		// From: https://github.com/dotnet/orleans/blob/main/src/AdoNet/Orleans.Persistence.AdoNet/PostgreSQL-Persistence.sql
		if (!TableExists(connection, "orleansstorage")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Persistence.sql");
		}

		// From https://github.com/dotnet/orleans/blob/main/src/AdoNet/Orleans.Reminders.AdoNet/PostgreSQL-Reminders.sql
		if (!TableExists(connection, "orleansreminderstable")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Reminders.sql");
		}

		// From https://github.com/dotnet/orleans/blame/main/src/AdoNet/Orleans.Streaming.AdoNet/PostgreSQL-Streaming.sql
		if (!TableExists(connection, "orleansstreammessage")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Streaming.sql");
		}

		// From https://github.com/bingtianyiyan/orleans/blob/dev_trans/src/AdoNet/Orleans.Transactions.AdoNet/PostgreSQL-Transactions.sql
		if (!TableExists(connection, "orleanstransactionkeytable")) {
			ExecuteSQLResourceFile(connection, "PostgreSQL-Transactions.sql");
		}
	}

	private static bool ExecuteBoolQuery(NpgsqlConnection connection, string query)
	{
		using var command = new NpgsqlCommand(query, connection);
		object result = command.ExecuteScalar();
		if (result != null && result != DBNull.Value) {
			return Convert.ToBoolean(result);
		}
		return false; // Or handle the null case as needed
	}

	private static bool TableExists(NpgsqlConnection connection, string tableName)
	{
		return ExecuteBoolQuery(connection, @"SELECT EXISTS ( SELECT FROM pg_tables WHERE tablename = '" + tableName + @"' )");
	}

	private static int ExecuteSQLResourceFile(NpgsqlConnection connection, string resourceName)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		using Stream resourceStream = assembly.GetManifestResourceStream("JumpStartCS.Orleans.Grains.Utils.SQL." + resourceName);
		using StreamReader reader = new StreamReader(resourceStream);
		string sqlText = reader.ReadToEnd();
		using NpgsqlCommand cmd = new NpgsqlCommand(sqlText, connection);
		return cmd.ExecuteNonQuery();
	}
}
