var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
                builder =>
                {
                    builder.WithOrigins(
                        "https://localhost:5002/")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

webapi.Bootup.Run(builder.Configuration);

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<webapi.CoopHub>("/CoopHub");

app.Run();