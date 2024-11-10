using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooperativa.Migrations
{
    /// <inheritdoc />
    public partial class inicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    clienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    apellidos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cedula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cliente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    inscripcion = table.Column<int>(type: "int", nullable: false),
                    correo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contraseña = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.clienteID);
                });

            migrationBuilder.CreateTable(
                name: "Pasivos",
                columns: table => new
                {
                    pasivoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costoPasivo = table.Column<double>(type: "real", nullable: false),
                    detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaPasivo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasivos", x => x.pasivoID);
                });

            migrationBuilder.CreateTable(
                name: "Socios",
                columns: table => new
                {
                    socioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    apellidos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cedula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    inscripcion = table.Column<int>(type: "int", nullable: false),
                    correo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contraseña = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socios", x => x.socioID);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    depositoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cantidadDeposito = table.Column<double>(type: "real", nullable: false),
                    fechaDeposito = table.Column<DateTime>(type: "datetime2", nullable: false),
                    detalleDeposito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    clientesclienteID = table.Column<int>(type: "int", nullable: true),
                    cliente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.depositoID);
                    table.ForeignKey(
                        name: "FK_Depositos_Clientes_clientesclienteID",
                        column: x => x.clientesclienteID,
                        principalTable: "Clientes",
                        principalColumn: "clienteID");
                });

            migrationBuilder.CreateTable(
                name: "PasivosClientes",
                columns: table => new
                {
                    pasivoID = table.Column<int>(type: "int", nullable: false) // ojo cambio de pasivo a pasivoID
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costoPasivo = table.Column<double>(type: "real", nullable: false),
                    detallePasivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaPasivo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    clientesclienteID = table.Column<int>(type: "int", nullable: true),
                    cliente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasivosClientes", x => x.pasivoID);
                    table.ForeignKey(
                        name: "FK_PasivosClientes_Clientes_clientesclienteID",
                        column: x => x.clientesclienteID,
                        principalTable: "Clientes",
                        principalColumn: "clienteID");
                });

            migrationBuilder.CreateTable(
                name: "Retiros",
                columns: table => new
                {
                    retiroID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cantidadRetiro = table.Column<double>(type: "real", nullable: false),
                    fechaRetiro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    detalleRetiro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    clientesclienteID = table.Column<int>(type: "int", nullable: true),
                    cliente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Retiros", x => x.retiroID);
                    table.ForeignKey(
                        name: "FK_Retiros_Clientes_clientesclienteID",
                        column: x => x.clientesclienteID,
                        principalTable: "Clientes",
                        principalColumn: "clienteID");
                });

            migrationBuilder.CreateTable(
                name: "Transferencias",
                columns: table => new
                {
                    transferenciaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cantidadTransferencia = table.Column<double>(type: "real", nullable: false),
                    fechaTransferencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    detalleTransferencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    clientesclienteID = table.Column<int>(type: "int", nullable: true),
                    cliente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transferencias", x => x.transferenciaID);
                    table.ForeignKey(
                        name: "FK_Transferencias_Clientes_clientesclienteID",
                        column: x => x.clientesclienteID,
                        principalTable: "Clientes",
                        principalColumn: "clienteID");
                });

            migrationBuilder.CreateTable(
                name: "Ahorros",
                columns: table => new
                {
                    ahorroID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    montoAhorro = table.Column<double>(type: "real", nullable: false),
                    comprobante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fechaAhorro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    detalleAhorro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sociossocioID = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ahorros", x => x.ahorroID);
                    table.ForeignKey(
                        name: "FK_Ahorros_Socios_sociossocioID",
                        column: x => x.sociossocioID,
                        principalTable: "Socios",
                        principalColumn: "socioID");
                });

            migrationBuilder.CreateTable(
                name: "Creditos",
                columns: table => new
                {
                    creditoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    montoCredito = table.Column<double>(type: "real", nullable: false),
                    fechaCredito = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tiempo = table.Column<int>(type: "int", nullable: false),
                    interes = table.Column<double>(type: "real", nullable: false),
                    cuota = table.Column<double>(type: "real", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalCredito = table.Column<double>(type: "real", nullable: false),
                    sociossocioID = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditos", x => x.creditoID);
                    table.ForeignKey(
                        name: "FK_Creditos_Socios_sociossocioID",
                        column: x => x.sociossocioID,
                        principalTable: "Socios",
                        principalColumn: "socioID");
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    eventoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tipoEvento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costoEvento = table.Column<double>(type: "real", nullable: false),
                    detalleEvento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lugar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sociossocioID = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.eventoID);
                    table.ForeignKey(
                        name: "FK_Eventos_Socios_sociossocioID",
                        column: x => x.sociossocioID,
                        principalTable: "Socios",
                        principalColumn: "socioID");
                });

            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    loginID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contraseña = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sociossocioID = table.Column<int>(type: "int", nullable: true),
                    clientesclienteID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.loginID);
                    table.ForeignKey(
                        name: "FK_Login_Clientes_clientesclienteID",
                        column: x => x.clientesclienteID,
                        principalTable: "Clientes",
                        principalColumn: "clienteID");
                    table.ForeignKey(
                        name: "FK_Login_Socios_sociossocioID",
                        column: x => x.sociossocioID,
                        principalTable: "Socios",
                        principalColumn: "socioID");
                });

            //migrationBuilder.CreateTable(
            //    name: "Bancos",
            //    columns: table => new
            //    {
            //        BancoID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        creditoscreditoID = table.Column<int>(type: "int", nullable: true),
            //        nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        interes = table.Column<double>(type: "real", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Bancos", x => x.BancoID);
            //        table.ForeignKey(
            //            name: "FK_Bancos_Creditos_creditoscreditoID",
            //            column: x => x.creditoscreditoID,
            //            principalTable: "Creditos",
            //            principalColumn: "creditoID");
            //    });

            migrationBuilder.CreateTable(
                name: "Utilidades",
                columns: table => new
                {
                    utilidadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    utilidadTotal = table.Column<double>(type: "real", nullable: false),
                    utilidadPorSocio = table.Column<double>(type: "real", nullable: false),
                    fechaUtilidad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sociossocioID = table.Column<int>(type: "int", nullable: true),
                    socio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    inscripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creditoscreditoID = table.Column<int>(type: "int", nullable: true),
                    interes = table.Column<double>(type: "real", nullable: false),
                    totalCredito = table.Column<double>(type: "real", nullable: false),
                    costoEvento = table.Column<double>(type: "real", nullable: false),
                    montoAhorro = table.Column<double>(type: "real", nullable: false),
                    costoPasivo = table.Column<double>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilidades", x => x.utilidadID);
                    table.ForeignKey(
                        name: "FK_Utilidades_Creditos_creditoscreditoID",
                        column: x => x.creditoscreditoID,
                        principalTable: "Creditos",
                        principalColumn: "creditoID");
                    table.ForeignKey(
                        name: "FK_Utilidades_Socios_sociossocioID",
                        column: x => x.sociossocioID,
                        principalTable: "Socios",
                        principalColumn: "socioID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ahorros_sociossocioID",
                table: "Ahorros",
                column: "sociossocioID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Bancos_creditoscreditoID",
            //    table: "Bancos",
            //    column: "creditoscreditoID");

            migrationBuilder.CreateIndex(
                name: "IX_Creditos_sociossocioID",
                table: "Creditos",
                column: "sociossocioID");

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_clientesclienteID",
                table: "Depositos",
                column: "clientesclienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_sociossocioID",
                table: "Eventos",
                column: "sociossocioID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_clientesclienteID",
                table: "Login",
                column: "clientesclienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_sociossocioID",
                table: "Login",
                column: "sociossocioID");

            migrationBuilder.CreateIndex(
                name: "IX_PasivosClientes_clientesclienteID",
                table: "PasivosClientes",
                column: "clientesclienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Retiros_clientesclienteID",
                table: "Retiros",
                column: "clientesclienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Transferencias_clientesclienteID",
                table: "Transferencias",
                column: "clientesclienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Utilidades_creditoscreditoID",
                table: "Utilidades",
                column: "creditoscreditoID");

            migrationBuilder.CreateIndex(
                name: "IX_Utilidades_sociossocioID",
                table: "Utilidades",
                column: "sociossocioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ahorros");

            //migrationBuilder.DropTable(
            //    name: "Bancos");

            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Login");

            migrationBuilder.DropTable(
                name: "Pasivos");

            migrationBuilder.DropTable(
                name: "PasivosClientes");

            migrationBuilder.DropTable(
                name: "Retiros");

            migrationBuilder.DropTable(
                name: "Transferencias");

            migrationBuilder.DropTable(
                name: "Utilidades");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Creditos");

            migrationBuilder.DropTable(
                name: "Socios");
        }
    }
}
