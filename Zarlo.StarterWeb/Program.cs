using Microsoft.AspNetCore;
using Serilog;

using Zarlo.StarterWeb;

try
{

    WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((b, c)=> {
            var env = b.HostingEnvironment;
        })
        .UseSerilog()
        .UseStartup<WebApp>()
        .Build()
        .Start();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

return 1;
