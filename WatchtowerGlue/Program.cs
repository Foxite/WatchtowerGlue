using WatchtowerGlue.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<NotificationService>();














var app = builder.Build();

app.UseHttpLogging();
app.MapControllers();

app.Run();
