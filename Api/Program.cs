using Api.Filters;
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

/* Swagger */

builder.Services.AddSwaggerGen(opt =>
{
    opt.CustomOperationIds(e =>
        $"{e.HttpMethod}_{e.RelativePath?.Replace("/", "_").Replace("{", "").Replace("}", "").Replace("$", "").Trim('_')}");
    opt.DocumentFilter<RemoveODataRoutes>();
    opt.MapType<Delta<Group>>(() => new OpenApiSchema
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["maintainerId"] = new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null }
        }
    });

    opt.MapType<Delta<License>>(() => new OpenApiSchema
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["type"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["pricePerSeat"] = new OpenApiSchema { Type = JsonSchemaType.Number },
            ["isProrated"] = new OpenApiSchema { Type = JsonSchemaType.Boolean },
            ["validFrom"] = new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date" },
            ["validTo"] = new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date" },
            ["seats"] = new OpenApiSchema { Type = JsonSchemaType.Integer },
            ["groupId"] = new OpenApiSchema { Type = JsonSchemaType.Integer },
            ["previousId"] = new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null }
        }
    });

    opt.MapType<Delta<Seat>>(() => new OpenApiSchema
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["licenseId"] = new OpenApiSchema { Type = JsonSchemaType.Integer },
            ["assignedToId"] = new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null },
            ["proratedPurchase"] = new OpenApiSchema { Type = JsonSchemaType.Boolean },
            ["validFrom"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "date" }
        }
    });

    opt.MapType<Delta<User>>(() => new OpenApiSchema
    {
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["username"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["email"] = new OpenApiSchema { Type = JsonSchemaType.String }
        }
    });
});

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();