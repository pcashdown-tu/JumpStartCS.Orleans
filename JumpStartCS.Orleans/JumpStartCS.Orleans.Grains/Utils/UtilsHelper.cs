using Microsoft.Extensions.Configuration;

/// <summary>
/// Allows static access to the app's injected services
/// </summary>
public class UtilsHelper
{
	public static IConfiguration? Configuration;

	public static void Initialize(IServiceProvider serviceProvider)
	{
		Configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
	}

	public static TConfig GetConfig<TConfig>(string key)
	{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
		return Configuration.GetSection(key).Get<TConfig>();
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
	}
}
