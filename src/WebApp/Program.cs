var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.MapGet("/hi", () =>
{
    return $"Hello {DateTime.Now}";
});
app.Run();