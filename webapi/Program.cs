var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddSignalR().AddNewtonsoftJsonProtocol();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "LocalCors",
        builder =>
        {
            builder.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("LocalCors");

try
{
    await webapi.Bootup.Run(builder.Configuration);
}
catch(Exception ex)
{
    Console.WriteLine($"Error on startup. {ex.Message}");
}
//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHealthChecks("/healthz");
app.MapControllers();
app.MapHub<webapi.CoopHub>("/CoopHub");

app.Run();