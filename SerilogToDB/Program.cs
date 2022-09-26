using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

IConfiguration configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

var builder = WebApplication.CreateBuilder(args);
string connectionString = configuration.GetConnectionString("SqlConnLogging");

var columnoptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn("UserName", SqlDbType.VarChar)
    }
};
LogContext.PushProperty("UserName", "Raju");
    builder.Host.UseSerilog((ctx, services, lc) => lc
    .ReadFrom.Configuration(configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.MSSqlServer(connectionString,
    sinkOptions: new MSSqlServerSinkOptions { TableName = "WebApiLogs" }
    , null, null, LogEventLevel.Error,null
    , columnOptions: columnoptions, null, null));


//builder.WebHost.UseConfiguration(configuration);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
