using AiSdk.Abstractions;
using BlazorServerExample;
using BlazorServerExample.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Mock Language Model for demonstration
builder.Services.AddSingleton<ILanguageModel, MockLanguageModel>();

// Alternative: Use OpenAI (uncomment to use real AI)
// builder.Services.AddOpenAI(options =>
// {
//     options.ApiKey = builder.Configuration["OpenAI:ApiKey"] ??
//         throw new InvalidOperationException("OpenAI API key not configured");
//     options.DefaultModel = "gpt-4";
// });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
