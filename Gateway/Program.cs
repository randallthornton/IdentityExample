using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultScheme = "Cookies";
    opts.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", opts =>
    {
        opts.Authority = builder.Configuration.GetValue<string>("Identity:Authority");
        opts.Scope.Add("profile");
        opts.Scope.Add("monolith");
        opts.GetClaimsFromUserInfoEndpoint = true;

        opts.ClientId = "gateway";
        opts.ClientSecret = "gatewaysecret";
        opts.ResponseType = "code";

        opts.SaveTokens = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/monolith/{*path}", async (string path, HttpContext context) =>
{
    var url = builder.Configuration.GetValue<string>("MonolithUrl");

    var token = await context.GetTokenAsync("access_token");
    
    var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var response = await httpClient.GetAsync($"{url}/{path}");

    var json = await response.Content.ReadAsStreamAsync();

    await json.CopyToAsync(context.Response.Body);

    context.Response.StatusCode = response.IsSuccessStatusCode ? (int)response.StatusCode : 500;
})
.RequireAuthorization();

app.MapGet("/logout", async (x) =>
{

    await x.SignOutAsync("oidc");
    await x.SignOutAsync("Cookies");
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
