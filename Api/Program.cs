using Api.Services;
using Database;
using Database.Dto;
using Database.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.Identity.Web;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDatabase(builder.Configuration);
builder.Services.AddSingleton<IIdempotencyKeyService, CacheIdempotencyKeyService>();
builder.Services.AddMemoryCache();

/* ODATA */

var modelBuilder = new ODataConventionModelBuilder();
var licenses = modelBuilder.EntitySet<License>("Licenses");
var users = modelBuilder.EntitySet<User>("Users");
var seats= modelBuilder.EntitySet<Seat>("Seats");
var groups = modelBuilder.EntitySet<Group>("Groups");
var costs = modelBuilder.EntitySet<LicenseCostDto>("LicenseCosts");
licenses.EntityType.Function("Cost").Returns<LicenseCostDto>();
licenses.EntityType.Collection.Function("Cost").ReturnsCollection<LicenseCostDto>();

builder.Services.AddControllers()
    .AddOData(opt => opt
        .AddRouteComponents("odata", modelBuilder.GetEdmModel())
        .Filter()
        .OrderBy()
        .Count()
        .Select()
        .Expand()
        .SetMaxTop(10)
    );

/* REST */

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.Services.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.Services.ClearDatabase();
    app.Services.SeedDatabase();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();