using BusinessLayer;
using Castle.DynamicProxy;
using DataLayer;
using Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IServiceLayer, ServiceLayer>();

// Register the WeatherForecast proxy
builder.Services.AddScoped<IWeatherForecast>(provider =>
{
    var weatherForecast = new WeatherForecast(); // Original implementation
    var interceptor = new MethodInterceptor();  // Interceptor instance
    return ProxyFactory.CreateProxy<IWeatherForecast>(weatherForecast, interceptor); // Wrap with proxy
});

// Add services to the container
builder.Services.AddScoped<IServiceLayer>(provider =>
{
    IWeatherForecast weatherForecast = provider.GetRequiredService<IWeatherForecast>(); // Get the proxy instance
    var serviceLayer = new ServiceLayer(weatherForecast); // Original implementation
    var interceptor = new MethodInterceptor();         // Interceptor instance
    return ProxyFactory.CreateProxy<IServiceLayer>(serviceLayer, interceptor); // Wrap with proxy
});




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
