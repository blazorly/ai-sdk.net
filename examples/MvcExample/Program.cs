using AiSdk.Abstractions;
using MvcExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register AI SDK services
// In production, you would configure a real provider here:
// builder.Services.AddSingleton<ILanguageModel>(sp =>
//     new OpenAIProvider(apiKey: "your-key").ChatModel("gpt-4"));
builder.Services.AddSingleton<ILanguageModel, MockLanguageModel>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();
