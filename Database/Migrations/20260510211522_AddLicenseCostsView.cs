using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseCostsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                     CREATE VIEW "LicenseCosts" AS
                                     SELECT 
                                         l."Id",
                                         l."Name",
                                         l."ValidFrom",
                                         l."ValidTo",
                                         COUNT(s."Id") AS "Seats",
                                         COALESCE(SUM(
                                             l."PricePerSeat" *
                                             CASE 
                                                 WHEN s."ProratedPurchase" = true 
                                                      AND l."ValidTo" > l."ValidFrom"
                                                 THEN 
                                                     GREATEST(0, (l."ValidTo"::date - s."ValidFrom"::date))::numeric
                                                     / NULLIF((l."ValidTo"::date - l."ValidFrom"::date), 0)
                                                 ELSE 
                                                     1
                                             END
                                         ), 0) AS "CurrentCost",
                                         COALESCE(SUM(l."PricePerSeat"), 0) AS "RenewalCost"
                                     FROM "Licenses" l
                                     LEFT JOIN "Seats" s ON s."LicenseId" = l."Id"
                                     GROUP BY l."Id", l."Name", l."ValidFrom", l."ValidTo", l."PricePerSeat"
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP VIEW "LicenseCosts";""");
        }

    }
}
