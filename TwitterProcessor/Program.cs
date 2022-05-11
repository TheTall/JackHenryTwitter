using DataLayer;
using System.Collections.Concurrent;
using TwitterProcessor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TwitterRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddHttpClient();

// Use a Queue for data
// This is a simple implementation that would be better to use RabbitMQ or Arure ServiceBus/Functions
builder.Services.AddSingleton<ConcurrentQueue<string>>();
builder.Services.AddSingleton<TwitterReader>();
builder.Services.AddHostedService<ReadTweetQueue>();
builder.Services.AddHostedService<QueuedWorker>();


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
