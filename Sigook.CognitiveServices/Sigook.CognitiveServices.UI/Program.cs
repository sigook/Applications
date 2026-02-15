using Microsoft.CognitiveServices.Speech;
using Sigook.CognitiveServices.Core.BussinesServices.Implementations;
using Sigook.CognitiveServices.Core.BussinesServices.Interfaces;
using Sigook.CognitiveServices.Core.Interfaces.Cloud;
using Sigook.CognitiveServices.Infraestructure.Cloud;
using Sigook.CognitiveServices.UI.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(opt =>
{
    var subscrition = builder.Configuration["SpeechConfiguration:SubscriptionKey"];
    var region = builder.Configuration["SpeechConfiguration:Region"];
    var config = SpeechConfig.FromSubscription(subscrition, region);
    builder.Configuration.Bind(config);
    return config;
});
builder.Services.AddSingleton<ISpeechConverter, SpeechConverter>();
builder.Services.AddScoped<ISpeechService, SpeechService>();
builder.Services.AddHealthChecks()
    .AddCheck<SpeechConfigurationHealthCheck>("speech-configuration");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
