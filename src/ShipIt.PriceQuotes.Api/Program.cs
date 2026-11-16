using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using ShipIt.PriceQuotes.Domain;
using ShipIt.PriceQuotes.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.Converters.Add( 
        new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IAuthorizationHandler, ShipItAuthHandler>();

builder.Services.AddScoped<IPriceQuoteService, PriceQuoteService>();
builder.Services.AddScoped<IPriceQuoteRepository, PriceQuoteRepository>();

builder.Services.Configure<PriceQuoteRepositoryOptions>(
        builder.Configuration.GetSection(
            nameof(PriceQuoteRepositoryOptions) 
        )
    );

builder.Services.AddAuthentication()
    .AddJwtBearer(options => 
    {
        options.Authority = "https://localhost:5001"; 
        options.TokenValidationParameters.ValidateAudience = false; 
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("QuoteReadPolicy", policy =>
        policy.Requirements.Add(
            new ShipItRequirement( 
                "shipit.pricequotes.api.read",
                "Quoter")))
    .AddPolicy("QuoteWritePolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope",
                "shipit.pricequotes.api.write");
        });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseRouting(); 
app.UseCors();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment()) 
    app.UseExceptionHandler("/error-development");
else
    app.UseExceptionHandler("/error");

app.Run();


public class ShipItRequirement(string claim, string role) : IAuthorizationRequirement
{
    public string Claim { get; } = claim;
    public string Role { get; } = role;
}

public class ShipItAuthHandler() : AuthorizationHandler<ShipItRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ShipItRequirement requirement)
    {

        var isRequiredClaimPresent = context.User.Claims.ToList().Exists(c => c.Value == requirement.Claim);
        var isUserSubPresent = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value is not null;

        // Systeem heeft rechten en er is geen user
        if (isRequiredClaimPresent && !isUserSubPresent)
            context.Succeed(requirement);

        // Systeem heeft rechten, er is een user en die heeft de nodige rol.
        else if (isRequiredClaimPresent && isUserSubPresent && context.User.IsInRole(requirement.Role))
            context.Succeed(requirement);

        // Alle andere gevallen
        else
            context.Fail();

        return Task.CompletedTask;
    }
}