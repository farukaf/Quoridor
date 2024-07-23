using Blazored.LocalStorage;
using Quoridor.Services;
using Quoridor.Components;
using Quoridor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// TODO: Make some extensions to organize before this file starting to become bloated
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
