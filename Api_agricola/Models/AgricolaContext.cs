using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api_agricola.Models;

public partial class AgricolaContext : DbContext
{
    

    public AgricolaContext(DbContextOptions<AgricolaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Finca> Fincas { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Finca>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Fincas__3213E83FC5E588D2");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Hectareas)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("hectareas");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(200)
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grupos__3213E83F2C9BB9EC");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdLote).HasColumnName("id_lote");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lotes__3213E83F7F1832E8");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Arboles).HasColumnName("arboles");
            entity.Property(e => e.Etapa)
                .HasMaxLength(100)
                .HasColumnName("etapa");
            entity.Property(e => e.IdFinca).HasColumnName("id_finca");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
