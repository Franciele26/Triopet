using MudBlazor.Services;
using Refit;
using Triopet.Web;
using Triopet.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    options.DetailedErrors = true;
});

// Adicionar MudBlazor
builder.Services.AddMudServices();

// Adicionar Refit (servi�o da API)
builder.Services
    .AddRefitClient<IApiService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7068"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
