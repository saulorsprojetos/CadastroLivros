using CadastroLivros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroLivros.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Livro> Livros => Set<Livro>();
        public DbSet<Autor> Autores => Set<Autor>();
        public DbSet<Assunto> Assuntos => Set<Assunto>();
        public DbSet<LivroAutor> LivroAutores => Set<LivroAutor>();
        public DbSet<LivroAssunto> LivroAssuntos => Set<LivroAssunto>();
        public DbSet<FormaCompra> FormasCompra => Set<FormaCompra>();
        public DbSet<LivroPreco> LivroPrecos => Set<LivroPreco>();

        public DbSet<VwRelatorioLivrosPorAutor> VwRelatorioLivrosPorAutor => Set<VwRelatorioLivrosPorAutor>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Livro>(e =>
            {
                e.ToTable("Livro");
                e.HasKey(x => x.Codl);
                e.Property(x => x.Titulo).HasColumnType("varchar(40)");
                e.Property(x => x.Editora).HasColumnType("varchar(40)");
                e.Property(x => x.AnoPublicacao).HasColumnType("varchar(4)");
            });

            mb.Entity<Autor>(e =>
            {
                e.ToTable("Autor");
                e.HasKey(x => x.CodAu);
                e.Property(x => x.Nome).HasColumnType("varchar(40)");
            });

            mb.Entity<Assunto>(e =>
            {
                e.ToTable("Assunto");
                e.HasKey(x => x.codAs);
                e.Property(x => x.Descricao).HasColumnType("varchar(20)");
            });

            mb.Entity<LivroAutor>(e =>
            {
                e.ToTable("Livro_Autor");
                e.HasKey(x => new { x.Livro_Codl, x.Autor_CodAu });

                e.HasOne(x => x.Livro)
                 .WithMany(l => l.Autores)
                 .HasForeignKey(x => x.Livro_Codl);

                e.HasOne(x => x.Autor)
                 .WithMany(a => a.Livros)
                 .HasForeignKey(x => x.Autor_CodAu);
            });

            mb.Entity<LivroAssunto>(e =>
            {
                e.ToTable("Livro_Assunto");
                e.HasKey(x => new { x.Livro_Codl, x.Assunto_codAs });

                e.HasOne(x => x.Livro)
                 .WithMany(l => l.Assuntos)
                 .HasForeignKey(x => x.Livro_Codl);

                e.HasOne(x => x.Assunto)
                 .WithMany(a => a.Livros)
                 .HasForeignKey(x => x.Assunto_codAs);
            });

            mb.Entity<FormaCompra>(e =>
            {
                e.ToTable("FormaCompra");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).HasColumnType("varchar(30)");
                e.HasIndex(x => x.Nome).IsUnique();
            });

            mb.Entity<LivroPreco>(e =>
            {
                e.ToTable("Livro_Preco");
                e.HasKey(x => new { x.Livro_Codl, x.FormaCompra_Id });
                e.Property(x => x.Valor).HasColumnType("decimal(10,2)");

                e.HasOne(x => x.Livro)
                 .WithMany(l => l.Precos)
                 .HasForeignKey(x => x.Livro_Codl);

                e.HasOne(x => x.FormaCompra)
                 .WithMany(f => f.Precos)
                 .HasForeignKey(x => x.FormaCompra_Id);
            });

            mb.Entity<VwRelatorioLivrosPorAutor>(e =>
            {
                e.HasNoKey();
                e.ToView("vw_RelatorioLivrosPorAutor");
            });
        }
    }
    public class VwRelatorioLivrosPorAutor
    {
        public int CodAu { get; set; }
        public string AutorNome { get; set; } = "";
        public int Codl { get; set; }
        public string Titulo { get; set; } = "";
        public string Editora { get; set; } = "";
        public int Edicao { get; set; }
        public string AnoPublicacao { get; set; } = "";
        public string? Assuntos { get; set; }
        public string? Precos { get; set; }
    }
}
