using Cbd.Api.HostedServices;
using Cbd.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderAggregationTask>();
builder.Services.AddHostedService<AggregatedOrdersInternalTask>();

builder.Services.AddSingleton<AggregatedOrdersChannel>();
builder.Services.AddSingleton<CreatedOrdersChannel>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
