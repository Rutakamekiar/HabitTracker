using System.Globalization;
using HabitTracker.Api.CommandActions;
using HabitTracker.Api.Constants;
using HabitTracker.Api.Controllers;
using HabitTracker.Api.TelegramWrappers;
using HabitTracker.Application.Services.Records;
using HabitTracker.Application.Services.TelegramUsers;
using HabitTracker.Infrastructure;
using HabitTracker.Infrastructure.Entities;
using HabitTracker.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")))
    .AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ITelegramUserService, TelegramUserService>();
builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddLogging();

builder.Services.AddLocalization();

builder.Services
    .AddScoped<ICommand, DefaultCommand>()
    .AddScoped<ICommand, StartCommand>()
    .AddScoped<ICommand, HelpCommand>()
    .AddScoped<ICommand, RulesCommand>()
    .AddScoped<ICommand, CancelCommand>()
    .AddScoped<ICommand, AddCategoryCommand>()
    .AddScoped<CommandProcessor>();

builder.Services.AddScoped<HabitTrackerTelegramClientWrapper>(_ =>
{
    var token = builder.Configuration["TelegramToken"] ?? throw new ArgumentException("Telegram token is not set");
    var telegramBotClient = new TelegramBotClient(token);
    return new HabitTrackerTelegramClientWrapper(telegramBotClient);
});

builder.Services.AddScoped<ZvychajnaTelegramClientWrapper>(_ =>
{
    var token = builder.Configuration["ZvychajnaTelegramToken"] ?? throw new ArgumentException("Telegram token is not set");
    var telegramBotClient = new TelegramBotClient(token);
    return new ZvychajnaTelegramClientWrapper(telegramBotClient);
});

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "HabitTracker.Api v1");
    });
}

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("uk") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("uk"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();