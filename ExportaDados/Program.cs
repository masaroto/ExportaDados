using ExportaDados.Helpers;
using ExportaDados.Interfaces;
using ExportaDados.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
//builder.Services.AddSingleton<SoupReqService>();
builder.Services.AddScoped<SoupReqService>();
builder.Services.AddTransient<IJsonUtils, JsonUtils>();
builder.Services.AddTransient<IXmlUtils, XmlUtils>();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<string>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "text"
    });
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
