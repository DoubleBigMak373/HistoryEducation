using Makar.EducationService.TelegramBot;
using Makar.HistoryEducation.Application;
using Makar.HistoryEducation.Application.Contracts;
using Makar.HistoryEducation.Domain;
using Makar.HistoryEducation.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateLogger();
try
{
    var services = new ServiceCollection();
    services.AddTransient<TelegramBotService>();

    services.AddTransient<IEducationServiceAppContracts, EducationAppServise>();
    services.AddTransient<IExerciseRepository, ExerciseRepository>();

    var sp = services.BuildServiceProvider();
    var service = sp.GetRequiredService<TelegramBotService>();

    await service.InitializeAsync();

    Console.ReadLine();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}