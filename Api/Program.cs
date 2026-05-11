using System.Text.Json.Serialization;
using Api.Requests;
using Api.Services;
using Database;
using Database.Dto;
using Database.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData.ModelBuilder;

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
licenses.EntityType.Function("History").ReturnsCollection<License>();

users.EntityType.Action("LicenseAssignments").ReturnsCollectionFromEntitySet(seats)
    .CollectionParameter<AssignMultipleLicensesRequest>("Ids");

licenses.EntityType.Collection.Action("Post").Parameter<NewLicenseRequest>("request");
users.EntityType.Collection.Action("Post").Parameter<NewUserRequest>("request");
groups.EntityType.Collection.Action("Post").Parameter<NewGroupRequests>("request");
seats.EntityType.Collection.Action("Post").Parameter<NewSeatRequest>("request");

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddOData(opt => opt
        .AddRouteComponents("odata", modelBuilder.GetEdmModel(), new DefaultODataBatchHandler())
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
app.UseODataBatching();
app.UseRouting();
app.MapControllers();


app.Run();