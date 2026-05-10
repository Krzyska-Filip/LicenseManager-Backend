using Api.Services;
using Licenses.Database;
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
modelBuilder.EntitySet<License>("Licenses");
modelBuilder.EntitySet<User>("Users");
modelBuilder.EntitySet<Seat>("Seats");
modelBuilder.EntitySet<Group>("Groups");

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
    app.Services.SeedDatabase();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();