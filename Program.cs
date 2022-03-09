using CryptoStashIdentity;
using Npgsql;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    if (Environment.GetEnvironmentVariable("SEED") != null)
    {
        Log.Information("Seeding database...");
        var config = app.Services.GetRequiredService<IConfiguration>();

        // Setup Entity Core connection to PostgreSQL.
        NpgsqlConnectionStringBuilder connBuilder;
        // Get connection string from user secrets.
        connBuilder = new NpgsqlConnectionStringBuilder(config.GetConnectionString("IdentityDb"));
        if (config["IdentityDb"] != null) connBuilder.Password = config["IdentityDb"];

        SeedData.EnsureSeedData(connBuilder.ConnectionString, config["AdminPassword"]);
        Log.Information("Done seeding database.");
        return;
    }

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}