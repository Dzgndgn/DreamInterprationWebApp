using DreamAI.Context;
using DreamAI.Models;
using DreamAI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// IChatClient'i konteynýra KAYDET
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var key = cfg["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAIKey yok");
    var model = cfg["OpenAI:Model"] ?? "gpt-4o-mini";

    return new OpenAIClient(key)
        .GetChatClient(model)
        .AsIChatClient();
});
builder.Services.AddScoped<ReadFromData>();


// Servislerini kaydet
builder.Services.AddScoped<Interpretaion>();
builder.Services.AddControllersWithViews();

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddDbContext<DreamDbContext>(options => options.UseSqlServer("Server = localhost; Database = DreamAI; Trusted_Connection = True; TrustServerCertificate = True;"));
builder.Services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<DreamDbContext>().AddDefaultTokenProviders();
var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var readData = scope.ServiceProvider.GetRequiredService<ReadFromData>();
    await readData.read();
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dream}/{action=New}/{id?}"
    );
//app.MapRazorPages();

app.Run();
