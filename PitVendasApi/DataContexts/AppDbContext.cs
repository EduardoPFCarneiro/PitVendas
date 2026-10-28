using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PitVendasApi.Identity;
using PitVendasApi.Models;
using PitVendasApi.Services;

namespace PitVendasApi.DataContexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly string? _vendedorId;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
            : base(options)
        {
            _vendedorId = currentUser.Id;
        }

        public DbSet<Estado> Estados => Set<Estado>();
        public DbSet<Cidade> Cidades => Set<Cidade>();
        public DbSet<Endereco> Enderecos => Set<Endereco>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Fabrica> Fabricas => Set<Fabrica>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Cor> Cores => Set<Cor>();
        public DbSet<Tamanho> Tamanhos => Set<Tamanho>();
        public DbSet<ProdutoCor> ProdutoCores => Set<ProdutoCor>();
        public DbSet<ProdutoTamanho> ProdutoTamanhos => Set<ProdutoTamanho>();
        public DbSet<Preco> Precos => Set<Preco>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();
        public DbSet<PedidoItemGrade> PedidoItemGrades => Set<PedidoItemGrade>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Estado>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
                e.Property(x => x.Sigla).HasMaxLength(2).IsRequired();
                e.HasIndex(x => x.Sigla).IsUnique();
            });

            builder.Entity<Cidade>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(120).IsRequired();
                e.HasOne(x => x.Estado)
                 .WithMany(x => x.Cidades)
                 .HasForeignKey(x => x.EstadoId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(x => new { x.EstadoId, x.Nome });
            });

            builder.Entity<Endereco>(e =>
            {
                e.Property(x => x.Logradouro).HasMaxLength(200);
                e.Property(x => x.Numero).HasMaxLength(20);
                e.Property(x => x.Complemento).HasMaxLength(120);
                e.Property(x => x.Bairro).HasMaxLength(120);
                e.Property(x => x.Cep).HasMaxLength(9);
                e.HasOne(x => x.Cidade)
                 .WithMany()
                 .HasForeignKey(x => x.CidadeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Cor>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(60).IsRequired();
                e.HasIndex(x => x.Nome).IsUnique();
            });

            builder.Entity<Tamanho>(e =>
            {
                e.Property(x => x.Codigo).HasMaxLength(10).IsRequired();
                e.HasIndex(x => x.Codigo).IsUnique();
            });

            builder.Entity<ApplicationUser>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                e.Property(x => x.Foto).HasMaxLength(300);
            });

            builder.Entity<Cliente>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                e.Property(x => x.RazaoSocial).HasMaxLength(200).IsRequired();
                e.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
                e.Property(x => x.InscricaoEstadual).HasMaxLength(20);
                e.Property(x => x.Email).HasMaxLength(200);
                e.Property(x => x.Telefone).HasMaxLength(20);
                e.Property(x => x.Foto).HasMaxLength(300);
                e.Property(x => x.Observacao).HasMaxLength(500);

                e.HasIndex(x => new { x.VendedorId, x.Cnpj }).IsUnique();

                e.HasOne(x => x.Vendedor)
                 .WithMany()
                 .HasForeignKey(x => x.VendedorId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Endereco)
                 .WithMany()
                 .HasForeignKey(x => x.EnderecoId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Fabrica>(e =>
            {
                e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                e.Property(x => x.RazaoSocial).HasMaxLength(200).IsRequired();
                e.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200);
                e.Property(x => x.Telefone).HasMaxLength(20);
                e.Property(x => x.Comissao).HasPrecision(5, 2);

                e.HasIndex(x => new { x.VendedorId, x.Cnpj }).IsUnique();

                e.HasOne(x => x.Vendedor)
                 .WithMany()
                 .HasForeignKey(x => x.VendedorId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Endereco)
                 .WithMany()
                 .HasForeignKey(x => x.EnderecoId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Produto>(e =>
            {
                e.Property(x => x.Codigo).HasMaxLength(60).IsRequired();
                e.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
                e.Property(x => x.Observacao).HasMaxLength(500);
                e.Property(x => x.Foto).HasMaxLength(300);

                e.HasIndex(x => new { x.FabricaId, x.Codigo }).IsUnique();

                e.HasOne(x => x.Fabrica)
                 .WithMany(x => x.Produtos)
                 .HasForeignKey(x => x.FabricaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProdutoCor>(e =>
            {
                e.HasIndex(x => new { x.ProdutoId, x.CorId }).IsUnique();

                e.HasOne(x => x.Produto)
                 .WithMany(x => x.Cores)
                 .HasForeignKey(x => x.ProdutoId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Cor)
                 .WithMany()
                 .HasForeignKey(x => x.CorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProdutoTamanho>(e =>
            {
                e.HasIndex(x => new { x.ProdutoId, x.TamanhoId }).IsUnique();

                e.HasOne(x => x.Produto)
                 .WithMany(x => x.Tamanhos)
                 .HasForeignKey(x => x.ProdutoId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Tamanho)
                 .WithMany()
                 .HasForeignKey(x => x.TamanhoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Preco>(e =>
            {
                e.Property(x => x.Codigo).HasMaxLength(30).IsRequired();
                e.Property(x => x.Valor).HasPrecision(10, 2);
                e.Property(x => x.Observacao).HasMaxLength(300);

                e.HasIndex(x => new { x.ProdutoId, x.Codigo }).IsUnique();

                e.HasOne(x => x.Produto)
                 .WithMany(x => x.Precos)
                 .HasForeignKey(x => x.ProdutoId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Pedido>(e =>
            {
                e.Property(x => x.ValorBruto).HasPrecision(12, 2);
                e.Property(x => x.PercentualDesconto).HasPrecision(5, 2);
                e.Property(x => x.Desconto).HasPrecision(12, 2);
                e.Property(x => x.ValorTotal).HasPrecision(12, 2);
                e.Property(x => x.TabelaPreco).HasMaxLength(30);
                e.Property(x => x.PercentualComissao).HasPrecision(5, 2);
                e.Property(x => x.ValorComissao).HasPrecision(12, 2);
                e.Property(x => x.Prazo).HasMaxLength(100);
                e.Property(x => x.Frete).HasMaxLength(100);
                e.Property(x => x.CondicaoPagamento).HasMaxLength(100);
                e.Property(x => x.Observacao).HasMaxLength(500);
                e.Property(x => x.Status).HasConversion<int>();

                e.HasIndex(x => new { x.VendedorId, x.Numero }).IsUnique();
                e.HasIndex(x => x.Data);

                e.HasOne(x => x.Cliente)
                 .WithMany(x => x.Pedidos)
                 .HasForeignKey(x => x.ClienteId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Fabrica)
                 .WithMany()
                 .HasForeignKey(x => x.FabricaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Vendedor)
                 .WithMany()
                 .HasForeignKey(x => x.VendedorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PedidoItem>(e =>
            {
                e.Property(x => x.DescricaoProduto).HasMaxLength(300).IsRequired();
                e.Property(x => x.CodigoProduto).HasMaxLength(60).IsRequired();
                e.Property(x => x.NomeCor).HasMaxLength(60).IsRequired();
                e.Property(x => x.ValorUnitario).HasPrecision(10, 2);
                e.Property(x => x.Subtotal).HasPrecision(12, 2);
                e.Property(x => x.Observacao).HasMaxLength(300);

                e.HasIndex(x => new { x.PedidoId, x.ProdutoId, x.CorId }).IsUnique();

                e.HasOne(x => x.Pedido)
                 .WithMany(x => x.Itens)
                 .HasForeignKey(x => x.PedidoId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Produto)
                 .WithMany()
                 .HasForeignKey(x => x.ProdutoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Cor)
                 .WithMany()
                 .HasForeignKey(x => x.CorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<PedidoItemGrade>(e =>
            {
                e.Property(x => x.CodigoTamanho).HasMaxLength(10).IsRequired();

                e.HasIndex(x => new { x.PedidoItemId, x.TamanhoId }).IsUnique();

                e.HasOne(x => x.PedidoItem)
                 .WithMany(x => x.Grade)
                 .HasForeignKey(x => x.PedidoItemId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Tamanho)
                 .WithMany()
                 .HasForeignKey(x => x.TamanhoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            AplicarIsolamentoPorVendedor(builder);

            SeedData.Apply(builder);
        }

        private void AplicarIsolamentoPorVendedor(ModelBuilder builder)
        {
            builder.Entity<Cliente>().HasQueryFilter(x => x.VendedorId == _vendedorId);
            builder.Entity<Fabrica>().HasQueryFilter(x => x.VendedorId == _vendedorId);
            builder.Entity<Pedido>().HasQueryFilter(x => x.VendedorId == _vendedorId);

            builder.Entity<Produto>().HasQueryFilter(x => x.Fabrica.VendedorId == _vendedorId);

            builder.Entity<Preco>()
                   .HasQueryFilter(x => x.Produto.Fabrica.VendedorId == _vendedorId);
            builder.Entity<ProdutoCor>()
                   .HasQueryFilter(x => x.Produto.Fabrica.VendedorId == _vendedorId);
            builder.Entity<ProdutoTamanho>()
                   .HasQueryFilter(x => x.Produto.Fabrica.VendedorId == _vendedorId);
            builder.Entity<PedidoItem>()
                   .HasQueryFilter(x => x.Pedido.VendedorId == _vendedorId);
            builder.Entity<PedidoItemGrade>()
                   .HasQueryFilter(x => x.PedidoItem.Pedido.VendedorId == _vendedorId);
        }
    }
}
