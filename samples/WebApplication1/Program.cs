using PowCapServer;
using PowCapServer.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddPowCapServer(options =>
{
    options.TypeConfigs = new Dictionary<string, PowCapConfig>
    {
        ["login"] = new PowCapConfig { ChallengeDifficulty = 5 }
    };
});
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = "localhost:6379"; // Redis server configuration
//    options.InstanceName = "PowCapServer:";
//});

var app = builder.Build();

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

app.MapRazorPages();
app.MapPowCapServer();
app.MapPost("/api/test-token", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync().ConfigureAwait(false);
    var data = System.Text.Json.JsonDocument.Parse(body);
    var token = data.RootElement.GetProperty("token").GetString();

    if (string.IsNullOrEmpty(token))
    {
        return Results.BadRequest(new { error = "Token is required" });
    }

    // 这里可以调用 ValidateCaptchaToken 验证 token
    var captchaService = context.RequestServices.GetRequiredService<ICaptchaService>();
    var isValid = await captchaService.ValidateCaptchaTokenAsync(token).ConfigureAwait(false);

    if (isValid)
    {
        return Results.Ok(new { message = "Token is valid", token });
    }
    else
    {
        return Results.BadRequest(new { error = "Invalid or expired token" });
    }
});

app.Run();
