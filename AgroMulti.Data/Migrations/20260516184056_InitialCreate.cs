using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroMulti.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadoEntrega",
                columns: table => new
                {
                    EstadoEntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoEntrega", x => x.EstadoEntregaId);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ProductoId);
                });

            migrationBuilder.CreateTable(
                name: "Productor",
                columns: table => new
                {
                    ProductorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productor", x => x.ProductorId);
                });

            migrationBuilder.CreateTable(
                name: "SubProducto",
                columns: table => new
                {
                    SubProductoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProducto", x => x.SubProductoId);
                    table.ForeignKey(
                        name: "FK_SubProducto_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId");
                });

            migrationBuilder.CreateTable(
                name: "Entrega",
                columns: table => new
                {
                    EntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroEntrega = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEntrega = table.Column<DateOnly>(type: "date", nullable: false),
                    ProductorId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    SubProductoId = table.Column<int>(type: "int", nullable: true),
                    EstadoEntregaId = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NombreConductor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kilos = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Cajas = table.Column<int>(type: "int", nullable: false),
                    Sacos = table.Column<int>(type: "int", nullable: false),
                    KilosSecos = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Pasillo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroAnaquel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Piso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrega", x => x.EntregaId);
                    table.ForeignKey(
                        name: "FK_Entrega_EstadoEntrega",
                        column: x => x.EstadoEntregaId,
                        principalTable: "EstadoEntrega",
                        principalColumn: "EstadoEntregaId");
                    table.ForeignKey(
                        name: "FK_Entrega_Producto",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "ProductoId");
                    table.ForeignKey(
                        name: "FK_Entrega_Productor",
                        column: x => x.ProductorId,
                        principalTable: "Productor",
                        principalColumn: "ProductorId");
                    table.ForeignKey(
                        name: "FK_Entrega_SubProducto",
                        column: x => x.SubProductoId,
                        principalTable: "SubProducto",
                        principalColumn: "SubProductoId");
                });

            migrationBuilder.CreateTable(
                name: "HistoricosEstadoEntrega",
                columns: table => new
                {
                    HistoricoEstadoEntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntregaId = table.Column<int>(type: "int", nullable: false),
                    EstadoEntregaId = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosEstadoEntrega", x => x.HistoricoEstadoEntregaId);
                    table.ForeignKey(
                        name: "FK_HistoricoEstadoEntrega_Entrega",
                        column: x => x.EntregaId,
                        principalTable: "Entrega",
                        principalColumn: "EntregaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricoEstadoEntrega_EstadoEntrega",
                        column: x => x.EstadoEntregaId,
                        principalTable: "EstadoEntrega",
                        principalColumn: "EstadoEntregaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entrega_EstadoEntregaId",
                table: "Entrega",
                column: "EstadoEntregaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrega_ProductoId",
                table: "Entrega",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrega_ProductorId",
                table: "Entrega",
                column: "ProductorId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrega_SubProductoId",
                table: "Entrega",
                column: "SubProductoId");

            migrationBuilder.CreateIndex(
                name: "UQ_Entrega_NumeroEntrega",
                table: "Entrega",
                column: "NumeroEntrega",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoEstadoEntrega_EntregaId",
                table: "HistoricosEstadoEntrega",
                column: "EntregaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoEstadoEntrega_FechaCambio",
                table: "HistoricosEstadoEntrega",
                column: "FechaCambio");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosEstadoEntrega_EstadoEntregaId",
                table: "HistoricosEstadoEntrega",
                column: "EstadoEntregaId");

            migrationBuilder.CreateIndex(
                name: "UQ_Productor_Codigo",
                table: "Productor",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProducto_ProductoId",
                table: "SubProducto",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricosEstadoEntrega");

            migrationBuilder.DropTable(
                name: "Entrega");

            migrationBuilder.DropTable(
                name: "EstadoEntrega");

            migrationBuilder.DropTable(
                name: "Productor");

            migrationBuilder.DropTable(
                name: "SubProducto");

            migrationBuilder.DropTable(
                name: "Producto");
        }
    }
}
