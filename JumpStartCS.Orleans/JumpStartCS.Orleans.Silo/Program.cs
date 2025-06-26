using JumpStartCS.Orleans.Grains.Filters;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Roshambofu.Utils;
using Orleans.Transactions.AdoNet.Hosting;

DotEnv.Load();

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(siloBuilder =>
{
	siloBuilder.UseAdoNetClustering(options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.Configure<ClusterOptions>(options =>
	{
		options.ClusterId = "JumpstartCSCluster";
		options.ServiceId = "JumpstartCSService";
	});

	siloBuilder.AddAdoNetGrainStorage("tableStorage", options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.UseAdoNetReminderService(options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.AddAdoNetTransactionalStateStorageAsDefault(options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.UseTransactions();

	siloBuilder.AddAdoNetStreams("StreamProvider", options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.AddAdoNetGrainStorage("PubSubStore", options =>
	{
		options.Invariant = "Npgsql";
		options.ConnectionString = DbUtils.GetConnectionString();
	});

	siloBuilder.AddIncomingGrainCallFilter<LoggingIncomingGrainCallFilter>();

	//siloBuilder.Configure<GrainCollectionOptions>(options =>
	//{
	//    options.CollectionQuantum = TimeSpan.FromSeconds(20);

	//    options.CollectionAge = TimeSpan.FromSeconds(20);
	//});

});

var app = builder.Build();

UtilsHelper.Initialize(app.Services);

DbUtils.SetupAdoNetGrainStorageTables();

await app.RunAsync();
