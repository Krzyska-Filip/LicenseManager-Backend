using Licenses.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Identity.Web;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDatabase(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

/* ODATA */

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<License>("Licenses");
modelBuilder.EntitySet<User>("Users");
modelBuilder.EntitySet<Seat>("Seats");
modelBuilder.EntitySet<Group>("Groups");

builder.Services.AddControllers()
    .AddOData(opt => opt
        .Filter()
        .OrderBy()
        .Count()
        .Select()
        .SetMaxTop(100));

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
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();