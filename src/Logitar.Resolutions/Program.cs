using Logitar.Cms.Core.Commands;
using Logitar.Cms.Infrastructure.Commands;
using MediatR;

namespace Logitar.Resolutions;

internal class Program
{
  private const string DefaultUniqueName = "admin";
  private const string DefaultPassword = "P@s$W0rD";
  private const string DefaultLocale = "fr";

  private static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfiguration configuration = builder.Configuration;

    Startup startup = new(configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    IServiceScope scope = application.Services.CreateScope();
    IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    await mediator.Send(new InitializeDatabaseCommand());

    string uniqueName = configuration.GetValue<string>("CMS_USERNAME") ?? DefaultUniqueName;
    string password = configuration.GetValue<string>("CMS_PASSWORD") ?? DefaultPassword;
    string defaultLocale = configuration.GetValue<string>("CMS_LOCALE") ?? DefaultLocale;
    await mediator.Send(new InitializeCmsCommand(uniqueName, password, defaultLocale));

    application.Run();
  }
}
