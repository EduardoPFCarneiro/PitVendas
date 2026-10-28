using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PitVendasApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    concurrency_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    foto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    user_name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_user_name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    normalized_email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    password_hash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    security_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    concurrency_stamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone_number_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    access_failed_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cores", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "estados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sigla = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estados", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tamanhos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ordem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tamanhos", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    claim_value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provider_display_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    login_provider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cidades",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cidades", x => x.id);
                    table.ForeignKey(
                        name: "fk_cidades_estados_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "enderecos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    logradouro = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    complemento = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bairro = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cep = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cidade_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enderecos", x => x.id);
                    table.ForeignKey(
                        name: "fk_enderecos_cidades_cidade_id",
                        column: x => x.cidade_id,
                        principalTable: "cidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    razao_social = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    inscricao_estadual = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    foto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    endereco_id = table.Column<int>(type: "int", nullable: true),
                    vendedor_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clientes", x => x.id);
                    table.ForeignKey(
                        name: "fk_clientes_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_clientes_users_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "fabricas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    razao_social = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comissao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    endereco_id = table.Column<int>(type: "int", nullable: true),
                    vendedor_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fabricas", x => x.id);
                    table.ForeignKey(
                        name: "fk_fabricas_enderecos_endereco_id",
                        column: x => x.endereco_id,
                        principalTable: "enderecos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fabricas_users_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    numero = table.Column<int>(type: "int", nullable: false),
                    data = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    valor_bruto = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    desconto = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    valor_total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    percentual_comissao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    valor_comissao = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    total_pecas = table.Column<int>(type: "int", nullable: false),
                    prazo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    frete = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    condicao_pagamento = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parcelas = table.Column<int>(type: "int", nullable: false),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enviado_em = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    cliente_id = table.Column<int>(type: "int", nullable: false),
                    fabrica_id = table.Column<int>(type: "int", nullable: false),
                    vendedor_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pedidos", x => x.id);
                    table.ForeignKey(
                        name: "fk_pedidos_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pedidos_fabricas_fabrica_id",
                        column: x => x.fabrica_id,
                        principalTable: "fabricas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pedidos_users_vendedor_id",
                        column: x => x.vendedor_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "produtos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descricao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observacao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    foto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    fabrica_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_produtos", x => x.id);
                    table.ForeignKey(
                        name: "fk_produtos_fabricas_fabrica_id",
                        column: x => x.fabrica_id,
                        principalTable: "fabricas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedido_itens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descricao_produto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codigo_produto = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nome_cor = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    quantidade = table.Column<int>(type: "int", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    observacao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pedido_id = table.Column<int>(type: "int", nullable: false),
                    produto_id = table.Column<int>(type: "int", nullable: false),
                    cor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pedido_itens", x => x.id);
                    table.ForeignKey(
                        name: "fk_pedido_itens_cores_cor_id",
                        column: x => x.cor_id,
                        principalTable: "cores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_pedido_itens_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pedido_itens_produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "precos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    observacao = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_alteracao = table.Column<DateOnly>(type: "date", nullable: false),
                    ativo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    produto_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_precos", x => x.id);
                    table.ForeignKey(
                        name: "fk_precos_produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "produto_cores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    produto_id = table.Column<int>(type: "int", nullable: false),
                    cor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_produto_cores", x => x.id);
                    table.ForeignKey(
                        name: "fk_produto_cores_cores_cor_id",
                        column: x => x.cor_id,
                        principalTable: "cores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_produto_cores_produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "produto_tamanhos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    produto_id = table.Column<int>(type: "int", nullable: false),
                    tamanho_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_produto_tamanhos", x => x.id);
                    table.ForeignKey(
                        name: "fk_produto_tamanhos_produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_produto_tamanhos_tamanhos_tamanho_id",
                        column: x => x.tamanho_id,
                        principalTable: "tamanhos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pedido_item_grades",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quantidade = table.Column<int>(type: "int", nullable: false),
                    codigo_tamanho = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pedido_item_id = table.Column<int>(type: "int", nullable: false),
                    tamanho_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pedido_item_grades", x => x.id);
                    table.ForeignKey(
                        name: "fk_pedido_item_grades_pedido_itens_pedido_item_id",
                        column: x => x.pedido_item_id,
                        principalTable: "pedido_itens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pedido_item_grades_tamanhos_tamanho_id",
                        column: x => x.tamanho_id,
                        principalTable: "tamanhos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "cores",
                columns: new[] { "id", "nome" },
                values: new object[,]
                {
                    { 1, "Verde" },
                    { 2, "Azul" },
                    { 3, "Preto" },
                    { 4, "Cinza" },
                    { 5, "Branco" },
                    { 6, "Vermelho" },
                    { 7, "Rosa" },
                    { 8, "Sortido" }
                });

            migrationBuilder.InsertData(
                table: "estados",
                columns: new[] { "id", "nome", "sigla" },
                values: new object[,]
                {
                    { 1, "Acre", "AC" },
                    { 2, "Alagoas", "AL" },
                    { 3, "Amazonas", "AM" },
                    { 4, "Amapá", "AP" },
                    { 5, "Bahia", "BA" },
                    { 6, "Ceará", "CE" },
                    { 7, "Distrito Federal", "DF" },
                    { 8, "Espírito Santo", "ES" },
                    { 9, "Goiás", "GO" },
                    { 10, "Maranhão", "MA" },
                    { 11, "Minas Gerais", "MG" },
                    { 12, "Mato Grosso do Sul", "MS" },
                    { 13, "Mato Grosso", "MT" },
                    { 14, "Pará", "PA" },
                    { 15, "Paraíba", "PB" },
                    { 16, "Pernambuco", "PE" },
                    { 17, "Piauí", "PI" },
                    { 18, "Paraná", "PR" },
                    { 19, "Rio de Janeiro", "RJ" },
                    { 20, "Rio Grande do Norte", "RN" },
                    { 21, "Rondônia", "RO" },
                    { 22, "Roraima", "RR" },
                    { 23, "Rio Grande do Sul", "RS" },
                    { 24, "Santa Catarina", "SC" },
                    { 25, "Sergipe", "SE" },
                    { 26, "São Paulo", "SP" },
                    { 27, "Tocantins", "TO" }
                });

            migrationBuilder.InsertData(
                table: "tamanhos",
                columns: new[] { "id", "codigo", "ordem" },
                values: new object[,]
                {
                    { 1, "PP", 1 },
                    { 2, "P", 2 },
                    { 3, "M", 3 },
                    { 4, "G", 4 },
                    { 5, "GG", 5 },
                    { 6, "XG", 6 },
                    { 7, "XGG", 7 },
                    { 8, "2", 8 },
                    { 9, "4", 9 },
                    { 10, "6", 10 },
                    { 11, "8", 11 },
                    { 12, "10", 12 },
                    { 13, "12", 13 },
                    { 14, "14", 14 },
                    { 15, "16", 15 }
                });

            migrationBuilder.InsertData(
                table: "cidades",
                columns: new[] { "id", "estado_id", "nome" },
                values: new object[,]
                {
                    { 1, 21, "Alta Floresta d'Oeste" },
                    { 2, 21, "Alto Alegre dos Parecis" },
                    { 3, 21, "Alto Paraíso" },
                    { 4, 21, "Alvorada d'Oeste" },
                    { 5, 21, "Ariquemes" },
                    { 6, 21, "Buritis" },
                    { 7, 21, "Cabixi" },
                    { 8, 21, "Cacaulândia" },
                    { 9, 21, "Cacoal" },
                    { 10, 21, "Campo Novo de Rondônia" },
                    { 11, 21, "Candeias do Jamari" },
                    { 12, 21, "Castanheiras" },
                    { 13, 21, "Cerejeiras" },
                    { 14, 21, "Chupinguaia" },
                    { 15, 21, "Colorado do Oeste" },
                    { 16, 21, "Corumbiara" },
                    { 17, 21, "Costa Marques" },
                    { 18, 21, "Cujubim" },
                    { 19, 21, "Espigão d'Oeste" },
                    { 20, 21, "Governador Jorge Teixeira" },
                    { 21, 21, "Guajará-Mirim" },
                    { 22, 21, "Itapuã do Oeste" },
                    { 23, 21, "Jaru" },
                    { 24, 21, "Ji-Paraná" },
                    { 25, 21, "Machadinho d'Oeste" },
                    { 26, 21, "Ministro Andreazza" },
                    { 27, 21, "Mirante da Serra" },
                    { 28, 21, "Monte Negro" },
                    { 29, 21, "Nova Brasilândia d'Oeste" },
                    { 30, 21, "Nova Mamoré" },
                    { 31, 21, "Nova União" },
                    { 32, 21, "Novo Horizonte do Oeste" },
                    { 33, 21, "Ouro Preto do Oeste" },
                    { 34, 21, "Parecis" },
                    { 35, 21, "Pimenta Bueno" },
                    { 36, 21, "Pimenteiras do Oeste" },
                    { 37, 21, "Porto Velho" },
                    { 38, 21, "Presidente Médici" },
                    { 39, 21, "Primavera de Rondônia" },
                    { 40, 21, "Rio Crespo" },
                    { 41, 21, "Rolim de Moura" },
                    { 42, 21, "Santa Luzia d'Oeste" },
                    { 43, 21, "São Felipe d'Oeste" },
                    { 44, 21, "São Francisco do Guaporé" },
                    { 45, 21, "São Miguel do Guaporé" },
                    { 46, 21, "Seringueiras" },
                    { 47, 21, "Teixeirópolis" },
                    { 48, 21, "Theobroma" },
                    { 49, 21, "Urupá" },
                    { 50, 21, "Vale do Anari" },
                    { 51, 21, "Vale do Paraíso" },
                    { 52, 21, "Vilhena" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cidades_estado_id_nome",
                table: "cidades",
                columns: new[] { "estado_id", "nome" });

            migrationBuilder.CreateIndex(
                name: "ix_clientes_endereco_id",
                table: "clientes",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_clientes_vendedor_id_cnpj",
                table: "clientes",
                columns: new[] { "vendedor_id", "cnpj" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cores_nome",
                table: "cores",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_cidade_id",
                table: "enderecos",
                column: "cidade_id");

            migrationBuilder.CreateIndex(
                name: "ix_estados_sigla",
                table: "estados",
                column: "sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fabricas_endereco_id",
                table: "fabricas",
                column: "endereco_id");

            migrationBuilder.CreateIndex(
                name: "ix_fabricas_vendedor_id_cnpj",
                table: "fabricas",
                columns: new[] { "vendedor_id", "cnpj" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pedido_item_grades_pedido_item_id_tamanho_id",
                table: "pedido_item_grades",
                columns: new[] { "pedido_item_id", "tamanho_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pedido_item_grades_tamanho_id",
                table: "pedido_item_grades",
                column: "tamanho_id");

            migrationBuilder.CreateIndex(
                name: "ix_pedido_itens_cor_id",
                table: "pedido_itens",
                column: "cor_id");

            migrationBuilder.CreateIndex(
                name: "ix_pedido_itens_pedido_id_produto_id_cor_id",
                table: "pedido_itens",
                columns: new[] { "pedido_id", "produto_id", "cor_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pedido_itens_produto_id",
                table: "pedido_itens",
                column: "produto_id");

            migrationBuilder.CreateIndex(
                name: "ix_pedidos_cliente_id",
                table: "pedidos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_pedidos_data",
                table: "pedidos",
                column: "data");

            migrationBuilder.CreateIndex(
                name: "ix_pedidos_fabrica_id",
                table: "pedidos",
                column: "fabrica_id");

            migrationBuilder.CreateIndex(
                name: "ix_pedidos_vendedor_id_numero",
                table: "pedidos",
                columns: new[] { "vendedor_id", "numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_precos_produto_id_codigo",
                table: "precos",
                columns: new[] { "produto_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_produto_cores_cor_id",
                table: "produto_cores",
                column: "cor_id");

            migrationBuilder.CreateIndex(
                name: "ix_produto_cores_produto_id_cor_id",
                table: "produto_cores",
                columns: new[] { "produto_id", "cor_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_produto_tamanhos_produto_id_tamanho_id",
                table: "produto_tamanhos",
                columns: new[] { "produto_id", "tamanho_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_produto_tamanhos_tamanho_id",
                table: "produto_tamanhos",
                column: "tamanho_id");

            migrationBuilder.CreateIndex(
                name: "ix_produtos_fabrica_id_codigo",
                table: "produtos",
                columns: new[] { "fabrica_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tamanhos_codigo",
                table: "tamanhos",
                column: "codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "pedido_item_grades");

            migrationBuilder.DropTable(
                name: "precos");

            migrationBuilder.DropTable(
                name: "produto_cores");

            migrationBuilder.DropTable(
                name: "produto_tamanhos");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "pedido_itens");

            migrationBuilder.DropTable(
                name: "tamanhos");

            migrationBuilder.DropTable(
                name: "cores");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "produtos");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "fabricas");

            migrationBuilder.DropTable(
                name: "enderecos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "cidades");

            migrationBuilder.DropTable(
                name: "estados");
        }
    }
}
