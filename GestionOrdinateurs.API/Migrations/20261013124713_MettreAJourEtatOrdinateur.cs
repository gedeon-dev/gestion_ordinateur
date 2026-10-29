using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class MettreAJourEtatOrdinateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Ordinateur\" SET \"Etat\" = 3 WHERE \"Etat\" = 2;");
            migrationBuilder.Sql("UPDATE \"Ordinateur\" SET \"Etat\" = 2 WHERE \"Etat\" = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Ordinateur\" SET \"Etat\" = 1 WHERE \"Etat\" = 2;");
            migrationBuilder.Sql("UPDATE \"Ordinateur\" SET \"Etat\" = 2 WHERE \"Etat\" = 3;");
        }
    }
}
