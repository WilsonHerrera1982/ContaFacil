﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContaFacil.Models
{
    public partial class ContableContext : DbContext
    {
        public ContableContext()
        {
        }

        public ContableContext(DbContextOptions<ContableContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<Comision> Comisions { get; set; } = null!;
        public virtual DbSet<ComisionContador> ComisionContadors { get; set; } = null!;
        public virtual DbSet<Cuentum> Cuenta { get; set; } = null!;
        public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; } = null!;
        public virtual DbSet<Emisor> Emisors { get; set; } = null!;
        public virtual DbSet<Empresa> Empresas { get; set; } = null!;
        public virtual DbSet<Factura> Facturas { get; set; } = null!;
        public virtual DbSet<Impuesto> Impuestos { get; set; } = null!;
        public virtual DbSet<Inventario> Inventarios { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<MenuPerfil> MenuPerfils { get; set; } = null!;
        public virtual DbSet<Pago> Pagos { get; set; } = null!;
        public virtual DbSet<Paquete> Paquetes { get; set; } = null!;
        public virtual DbSet<PaqueteContador> PaqueteContadors { get; set; } = null!;
        public virtual DbSet<Perfil> Perfils { get; set; } = null!;
        public virtual DbSet<Persona> Personas { get; set; } = null!;
        public virtual DbSet<Producto> Productos { get; set; } = null!;
        public virtual DbSet<ProductoProveedor> ProductoProveedors { get; set; } = null!;
        public virtual DbSet<Proveedor> Proveedors { get; set; } = null!;
        public virtual DbSet<TipoIdentificacion> TipoIdentificacions { get; set; } = null!;
        public virtual DbSet<TipoPago> TipoPagos { get; set; } = null!;
        public virtual DbSet<TipoTransaccion> TipoTransaccions { get; set; } = null!;
        public virtual DbSet<Tipocuentum> Tipocuenta { get; set; } = null!;
        public virtual DbSet<Transaccion> Transaccions { get; set; } = null!;
        public virtual DbSet<UnidadMedidum> UnidadMedida { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<UsuarioPerfil> UsuarioPerfils { get; set; } = null!;
        public virtual DbSet<VentaPaquete> VentaPaquetes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Database=contable;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoriaProducto>(entity =>
            {
                entity.HasKey(e => e.IdCategoriaProducto)
                    .HasName("categoria_producto_pkey");

                entity.ToTable("categoria_producto");

                entity.Property(e => e.IdCategoriaProducto)
                    .HasColumnName("id_categoria_producto")
                    .HasDefaultValueSql("nextval('seq_categoria_producto_id'::regclass)");

                entity.Property(e => e.Descripcion).HasColumnName("descripcion");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente)
                    .HasName("cliente_pkey");

                entity.ToTable("cliente");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cliente");

                entity.Property(e => e.IdCliente)
                    .HasColumnName("id_cliente")
                    .HasDefaultValueSql("nextval('seq_cliente'::regclass)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdPersona).HasColumnName("id_persona");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_cliente");

                entity.HasOne(d => d.IdPersonaNavigation)
                    .WithMany(p => p.Clientes)
                    .HasForeignKey(d => d.IdPersona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cliente_id_persona_fkey");
            });

            modelBuilder.Entity<Comision>(entity =>
            {
                entity.HasKey(e => e.IdComision)
                    .HasName("comision_pkey");

                entity.ToTable("comision");

                entity.Property(e => e.IdComision)
                    .HasColumnName("id_comision")
                    .HasDefaultValueSql("nextval('seq_comision'::regclass)");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.Property(e => e.Valor)
                    .HasPrecision(15, 2)
                    .HasColumnName("valor");

                entity.HasOne(d => d.IdPaqueteNavigation)
                    .WithMany(p => p.Comisions)
                    .HasForeignKey(d => d.IdPaquete)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comision_id_paquete_fkey");
            });

            modelBuilder.Entity<ComisionContador>(entity =>
            {
                entity.HasKey(e => e.IdComsionContador)
                    .HasName("comision_contador_pkey");

                entity.ToTable("comision_contador");

                entity.Property(e => e.IdComsionContador)
                    .HasColumnName("id_comsion_contador")
                    .HasDefaultValueSql("nextval('seq_comision_contador'::regclass)");

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasColumnName("estado");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.IdComision).HasColumnName("id_comision");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.Property(e => e.Valor)
                    .HasPrecision(15, 2)
                    .HasColumnName("valor");

                entity.HasOne(d => d.IdComisionNavigation)
                    .WithMany(p => p.ComisionContadors)
                    .HasForeignKey(d => d.IdComision)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comision_contador_id_comision_fkey");
            });

            modelBuilder.Entity<Cuentum>(entity =>
            {
                entity.HasKey(e => e.IdCuenta)
                    .HasName("cuenta_pkey");

                entity.ToTable("cuenta");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cuenta");

                entity.Property(e => e.IdCuenta)
                    .HasColumnName("id_cuenta")
                    .HasDefaultValueSql("nextval('seq_cuenta'::regclass)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdTipoCuenta).HasColumnName("id_tipo_cuenta");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.SaldoActual)
                    .HasPrecision(15, 2)
                    .HasColumnName("saldo_actual");

                entity.Property(e => e.SaldoInicial)
                    .HasPrecision(15, 2)
                    .HasColumnName("saldo_inicial");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Cuenta)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_cuenta");

                entity.HasOne(d => d.IdTipoCuentaNavigation)
                    .WithMany(p => p.Cuenta)
                    .HasForeignKey(d => d.IdTipoCuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cuenta_id_tipo_cuenta_fkey");
            });

            modelBuilder.Entity<DetalleFactura>(entity =>
            {
                entity.HasKey(e => e.IdDetalleFactura)
                    .HasName("detalle_factura_pkey");

                entity.ToTable("detalle_factura");

                entity.HasIndex(e => e.IdProducto, "fki_p");

                entity.Property(e => e.IdDetalleFactura)
                    .HasColumnName("id_detalle_factura")
                    .HasDefaultValueSql("nextval('seq_detalle_factura'::regclass)");

                entity.Property(e => e.Cantidad).HasColumnName("cantidad");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdFactura).HasColumnName("id_factura");

                entity.Property(e => e.IdProducto).HasColumnName("id_producto");

                entity.Property(e => e.PrecioUnitario)
                    .HasPrecision(15, 2)
                    .HasColumnName("precio_unitario");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdFacturaNavigation)
                    .WithMany(p => p.DetalleFacturas)
                    .HasForeignKey(d => d.IdFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("detalle_factura_id_factura_fkey");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.DetalleFacturas)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("detalle_producto");
            });

            modelBuilder.Entity<Emisor>(entity =>
            {
                entity.HasKey(e => e.IdEmisor)
                    .HasName("emisor_pkey");

                entity.ToTable("emisor");

                entity.Property(e => e.IdEmisor)
                    .HasColumnName("id_emisor")
                    .HasDefaultValueSql("nextval('seq_emisor'::regclass)");

                entity.Property(e => e.CertificadoDigital)
                    .HasMaxLength(100)
                    .HasColumnName("certificado_digital");

                entity.Property(e => e.Clave)
                    .HasMaxLength(100)
                    .HasColumnName("clave");

                entity.Property(e => e.CorreoElectronico)
                    .HasMaxLength(100)
                    .HasColumnName("correo_electronico");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .HasColumnName("direccion");

                entity.Property(e => e.Establecimiento)
                    .HasMaxLength(10)
                    .HasColumnName("establecimiento");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.NombreComercial)
                    .HasMaxLength(100)
                    .HasColumnName("nombre_comercial");

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(50)
                    .HasColumnName("nombre_usuario");

                entity.Property(e => e.ObligadoContabilidad)
                    .HasMaxLength(100)
                    .HasColumnName("obligado_contabilidad");

                entity.Property(e => e.PuntoEmision)
                    .HasMaxLength(10)
                    .HasColumnName("punto_emision");

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(100)
                    .HasColumnName("razon_social");

                entity.Property(e => e.Ruc)
                    .HasMaxLength(13)
                    .HasColumnName("ruc");

                entity.Property(e => e.Secuencial)
                    .HasMaxLength(50)
                    .HasColumnName("secuencial");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .HasColumnName("telefono");

                entity.Property(e => e.TipoAmbiente)
                    .HasMaxLength(1)
                    .HasColumnName("tipo_ambiente");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Emisors)
                    .HasForeignKey(d => d.IdEmpresa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("emisor_id_empresa_fkey");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.Emisors)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("emisor_id_usuario_fkey");
            });

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.HasKey(e => e.IdEmpresa)
                    .HasName("empresa_pkey");

                entity.ToTable("empresa");

                entity.Property(e => e.IdEmpresa)
                    .HasColumnName("id_empresa")
                    .HasDefaultValueSql("nextval('seq_empresa'::regclass)");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .HasColumnName("direccion");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(100)
                    .HasColumnName("identificacion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(100)
                    .HasColumnName("telefono");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasKey(e => e.IdFactura)
                    .HasName("factura_pkey");

                entity.ToTable("factura");

                entity.Property(e => e.IdFactura)
                    .HasColumnName("id_factura")
                    .HasDefaultValueSql("nextval('seq_factura'::regclass)");

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasColumnName("estado");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.MontoTotal)
                    .HasPrecision(15, 2)
                    .HasColumnName("monto_total");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Facturas)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("factura_id_cliente_fkey");
            });

            modelBuilder.Entity<Impuesto>(entity =>
            {
                entity.HasKey(e => e.IdImpuesto)
                    .HasName("impuesto_pkey");

                entity.ToTable("impuesto");

                entity.Property(e => e.IdImpuesto)
                    .HasColumnName("id_impuesto")
                    .HasDefaultValueSql("nextval('seq_impuesto_id'::regclass)");

                entity.Property(e => e.CodigoPorcentajeSri)
                    .HasColumnType("character varying")
                    .HasColumnName("codigo_porcentaje_sri");

                entity.Property(e => e.CodigoSri)
                    .HasMaxLength(10)
                    .HasColumnName("codigo_sri");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .HasColumnName("nombre");

                entity.Property(e => e.Porcentaje)
                    .HasPrecision(5, 2)
                    .HasColumnName("porcentaje");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<Inventario>(entity =>
            {
                entity.HasKey(e => e.IdInventario)
                    .HasName("inventario_pkey");

                entity.ToTable("inventario");

                entity.Property(e => e.IdInventario)
                    .HasColumnName("id_inventario")
                    .HasDefaultValueSql("nextval('seq_inventario_id'::regclass)");

                entity.Property(e => e.Cantidad)
                    .HasPrecision(10, 2)
                    .HasColumnName("cantidad");

                entity.Property(e => e.Descripcion).HasColumnName("descripcion");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaMovimiento)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_movimiento")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdProducto).HasColumnName("id_producto");

                entity.Property(e => e.NumeroDespacho)
                    .HasMaxLength(50)
                    .HasColumnName("numero_despacho");

                entity.Property(e => e.TipoMovimiento)
                    .HasMaxLength(1)
                    .HasColumnName("tipo_movimiento");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.Inventarios)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("inventario_id_producto_fkey");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.IdMenu)
                    .HasName("menu_pkey");

                entity.ToTable("menu");

                entity.Property(e => e.IdMenu)
                    .HasColumnName("id_menu")
                    .HasDefaultValueSql("nextval('seq_menu'::regclass)");

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .HasColumnName("action");

                entity.Property(e => e.Controller)
                    .HasMaxLength(50)
                    .HasColumnName("controller");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.Url)
                    .HasMaxLength(100)
                    .HasColumnName("url");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<MenuPerfil>(entity =>
            {
                entity.HasKey(e => e.IdMenuPerfil)
                    .HasName("menu_perfil_pkey");

                entity.ToTable("menu_perfil");

                entity.Property(e => e.IdMenuPerfil)
                    .HasColumnName("id_menu_perfil")
                    .HasDefaultValueSql("nextval('seq_menu_perfil'::regclass)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdMenu).HasColumnName("id_menu");

                entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdMenuNavigation)
                    .WithMany(p => p.MenuPerfils)
                    .HasForeignKey(d => d.IdMenu)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("menu_perfil_id_menu_fkey");

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.MenuPerfils)
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("menu_perfil_id_perfil_fkey");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.IdPago)
                    .HasName("pago_pkey");

                entity.ToTable("pago");

                entity.Property(e => e.IdPago)
                    .HasColumnName("id_pago")
                    .HasDefaultValueSql("nextval('seq_pago'::regclass)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdFactura).HasColumnName("id_factura");

                entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");

                entity.Property(e => e.Monto)
                    .HasPrecision(15, 2)
                    .HasColumnName("monto");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdFacturaNavigation)
                    .WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.IdFactura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pago_id_factura_fkey");

                entity.HasOne(d => d.IdTipoPagoNavigation)
                    .WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.IdTipoPago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pago_id_tipo_pago_fkey");
            });

            modelBuilder.Entity<Paquete>(entity =>
            {
                entity.HasKey(e => e.IdPaquete)
                    .HasName("paquete_pkey");

                entity.ToTable("paquete");

                entity.Property(e => e.IdPaquete)
                    .HasColumnName("id_paquete")
                    .HasDefaultValueSql("nextval('seq_paquete'::regclass)");

                entity.Property(e => e.CantidadEmisores).HasColumnName("cantidad_emisores");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .HasColumnName("descripcion");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .HasColumnName("nombre");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.Property(e => e.Valor)
                    .HasPrecision(15, 2)
                    .HasColumnName("valor");
            });

            modelBuilder.Entity<PaqueteContador>(entity =>
            {
                entity.HasKey(e => e.IdPaqueteContador)
                    .HasName("paquete_contador_pkey");

                entity.ToTable("paquete_contador");

                entity.Property(e => e.IdPaqueteContador)
                    .HasColumnName("id_paquete_contador")
                    .HasDefaultValueSql("nextval('seq_paquete_contador'::regclass)");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdPaqueteNavigation)
                    .WithMany(p => p.PaqueteContadors)
                    .HasForeignKey(d => d.IdPaquete)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("paquete_contador_id_paquete_fkey");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.PaqueteContadors)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("paquete_contador_id_usuario_fkey");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.IdPerfil)
                    .HasName("perfil_pkey");

                entity.ToTable("perfil");

                entity.Property(e => e.IdPerfil)
                    .HasColumnName("id_perfil")
                    .HasDefaultValueSql("nextval('seq_perfil'::regclass)");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.IdPersona)
                    .HasName("persona_pkey");

                entity.ToTable("persona");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_persona");

                entity.Property(e => e.IdPersona)
                    .HasColumnName("id_persona")
                    .HasDefaultValueSql("nextval('seq_persona'::regclass)");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(255)
                    .HasColumnName("direccion");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.Identificacion)
                    .HasMaxLength(13)
                    .HasColumnName("identificacion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .HasColumnName("telefono");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Personas)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_persona");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdProducto)
                    .HasName("producto_pkey");

                entity.ToTable("producto");

                entity.HasIndex(e => e.IdEmpresa, "fki_fk_producto_empresa");

                entity.HasIndex(e => e.IdImpuesto, "fki_i");

                entity.HasIndex(e => e.Codigo, "producto_codigo_key")
                    .IsUnique();

                entity.Property(e => e.IdProducto)
                    .HasColumnName("id_producto")
                    .HasDefaultValueSql("nextval('seq_producto_id'::regclass)");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(50)
                    .HasColumnName("codigo");

                entity.Property(e => e.Descripcion).HasColumnName("descripcion");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdCategoriaProducto).HasColumnName("id_categoria_producto");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdImpuesto).HasColumnName("id_impuesto");

                entity.Property(e => e.IdUnidadMedida).HasColumnName("id_unidad_medida");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.PrecioUnitario)
                    .HasPrecision(10, 2)
                    .HasColumnName("precio_unitario");

                entity.Property(e => e.Stock)
                    .HasPrecision(10, 2)
                    .HasColumnName("stock")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdCategoriaProductoNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdCategoriaProducto)
                    .HasConstraintName("producto_id_categoria_producto_fkey");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("fk_producto_empresa");

                entity.HasOne(d => d.IdImpuestoNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdImpuesto)
                    .HasConstraintName("fk_producto_impuesto");

                entity.HasOne(d => d.IdUnidadMedidaNavigation)
                    .WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdUnidadMedida)
                    .HasConstraintName("producto_id_unidad_medida_fkey");
            });

            modelBuilder.Entity<ProductoProveedor>(entity =>
            {
                entity.HasKey(e => e.IdProductoProveedor)
                    .HasName("producto_proveedor_pkey");

                entity.ToTable("producto_proveedor");

                entity.Property(e => e.IdProductoProveedor)
                    .HasColumnName("id_producto_proveedor")
                    .HasDefaultValueSql("nextval('seq_producto_proveedor_id'::regclass)");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdProducto).HasColumnName("id_producto");

                entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");

                entity.Property(e => e.PrecioCompra)
                    .HasPrecision(10, 2)
                    .HasColumnName("precio_compra");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.ProductoProveedors)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("producto_proveedor_id_producto_fkey");
            });

            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.IdProveedor)
                    .HasName("proveedor_pkey");

                entity.ToTable("proveedor");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_proveedor");

                entity.Property(e => e.IdProveedor)
                    .HasColumnName("id_proveedor")
                    .HasDefaultValueSql("nextval('seq_proveedor'::regclass)");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(255)
                    .HasColumnName("direccion");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .HasColumnName("telefono");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Proveedors)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_proveedor");
            });

            modelBuilder.Entity<TipoIdentificacion>(entity =>
            {
                entity.HasKey(e => e.IdTipoIdemtificacion)
                    .HasName("tipo_identificacion_pkey");

                entity.ToTable("tipo_identificacion");

                entity.Property(e => e.IdTipoIdemtificacion)
                    .HasColumnName("id_tipo_idemtificacion")
                    .HasDefaultValueSql("nextval('seq_tipo_identificacion'::regclass)");

                entity.Property(e => e.CodigoSri).HasColumnName("codigo_sri");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .HasColumnName("descripcion");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<TipoPago>(entity =>
            {
                entity.HasKey(e => e.IdTipoPago)
                    .HasName("tipo_pago_pkey");

                entity.ToTable("tipo_pago");

                entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<TipoTransaccion>(entity =>
            {
                entity.HasKey(e => e.IdTipoTransaccion)
                    .HasName("tipo_transaccion_pkey");

                entity.ToTable("tipo_transaccion");

                entity.Property(e => e.IdTipoTransaccion).HasColumnName("id_tipo_transaccion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Tipocuentum>(entity =>
            {
                entity.HasKey(e => e.IdTipoCuenta)
                    .HasName("tipocuenta_pkey");

                entity.ToTable("tipocuenta");

                entity.Property(e => e.IdTipoCuenta).HasColumnName("id_tipo_cuenta");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Transaccion>(entity =>
            {
                entity.HasKey(e => e.IdTransaccion)
                    .HasName("transaccion_pkey");

                entity.ToTable("transaccion");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_transaccion");

                entity.Property(e => e.IdTransaccion)
                    .HasColumnName("id_transaccion")
                    .HasDefaultValueSql("nextval('seq_transaccion'::regclass)");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Fecha).HasColumnName("fecha");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdTipoTransaccion).HasColumnName("id_tipo_transaccion");

                entity.Property(e => e.Monto)
                    .HasPrecision(15, 2)
                    .HasColumnName("monto");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdCuentaNavigation)
                    .WithMany(p => p.Transaccions)
                    .HasForeignKey(d => d.IdCuenta)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("transaccion_id_cuenta_fkey");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Transaccions)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_transaccion");

                entity.HasOne(d => d.IdTipoTransaccionNavigation)
                    .WithMany(p => p.Transaccions)
                    .HasForeignKey(d => d.IdTipoTransaccion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("transaccion_id_tipo_transaccion_fkey");
            });

            modelBuilder.Entity<UnidadMedidum>(entity =>
            {
                entity.HasKey(e => e.IdUnidadMedida)
                    .HasName("unidad_medida_pkey");

                entity.ToTable("unidad_medida");

                entity.Property(e => e.IdUnidadMedida)
                    .HasColumnName("id_unidad_medida")
                    .HasDefaultValueSql("nextval('seq_unidad_medida_id'::regclass)");

                entity.Property(e => e.Abreviatura)
                    .HasMaxLength(10)
                    .HasColumnName("abreviatura");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .HasColumnName("nombre");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("usuario_pkey");

                entity.ToTable("usuario");

                entity.HasIndex(e => e.IdEmpresa, "fki_empresa_usuario");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario")
                    .HasDefaultValueSql("nextval('seq_usuario'::regclass)");

                entity.Property(e => e.Clave)
                    .HasMaxLength(100)
                    .HasColumnName("clave");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");

                entity.Property(e => e.IdPersona).HasColumnName("id_persona");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .HasColumnName("nombre");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdEmpresaNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdEmpresa)
                    .HasConstraintName("empresa_usuario");

                entity.HasOne(d => d.IdPersonaNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdPersona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usuario_id_persona_fkey");
            });

            modelBuilder.Entity<UsuarioPerfil>(entity =>
            {
                entity.HasKey(e => e.IdUsuarioPerfil)
                    .HasName("usuario_perfil_pkey");

                entity.ToTable("usuario_perfil");

                entity.Property(e => e.IdUsuarioPerfil)
                    .HasColumnName("id_usuario_perfil")
                    .HasDefaultValueSql("nextval('seq_usuario_perfil'::regclass)");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.UsuarioPerfils)
                    .HasForeignKey(d => d.IdPerfil)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usuario_perfil_id_perfil_fkey");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.UsuarioPerfils)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usuario_perfil_id_usuario_fkey");
            });

            modelBuilder.Entity<VentaPaquete>(entity =>
            {
                entity.HasKey(e => e.IdVentaPaquete)
                    .HasName("venta_paquete_pkey");

                entity.ToTable("venta_paquete");

                entity.Property(e => e.IdVentaPaquete)
                    .HasColumnName("id_venta_paquete")
                    .HasDefaultValueSql("nextval('seq_venta_paquete'::regclass)");

                entity.Property(e => e.EstadoBoolean)
                    .IsRequired()
                    .HasColumnName("estado_boolean")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaModificacion)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_modificacion");

                entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

                entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

                entity.HasOne(d => d.IdPaqueteNavigation)
                    .WithMany(p => p.VentaPaquetes)
                    .HasForeignKey(d => d.IdPaquete)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("venta_paquete_id_paquete_fkey");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.VentaPaquetes)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("venta_paquete_id_usuario_fkey");
            });

            modelBuilder.HasSequence("seq_categoria_producto_id");

            modelBuilder.HasSequence("seq_cliente");

            modelBuilder.HasSequence("seq_comision");

            modelBuilder.HasSequence("seq_comision_contador");

            modelBuilder.HasSequence("seq_cuenta");

            modelBuilder.HasSequence("seq_detalle_factura");

            modelBuilder.HasSequence("seq_emisor");

            modelBuilder.HasSequence("seq_empresa");

            modelBuilder.HasSequence("seq_factura");

            modelBuilder.HasSequence("seq_impuesto_id");

            modelBuilder.HasSequence("seq_inventario_id");

            modelBuilder.HasSequence("seq_menu");

            modelBuilder.HasSequence("seq_menu_perfil");

            modelBuilder.HasSequence("seq_pago");

            modelBuilder.HasSequence("seq_paquete");

            modelBuilder.HasSequence("seq_paquete_contador");

            modelBuilder.HasSequence("seq_perfil");

            modelBuilder.HasSequence("seq_persona");

            modelBuilder.HasSequence("seq_producto_id");

            modelBuilder.HasSequence("seq_producto_proveedor_id");

            modelBuilder.HasSequence("seq_proveedor");

            modelBuilder.HasSequence("seq_tipo_cuenta");

            modelBuilder.HasSequence("seq_tipo_identificacion");

            modelBuilder.HasSequence("seq_tipo_pago");

            modelBuilder.HasSequence("seq_tipo_transaccion");

            modelBuilder.HasSequence("seq_transaccion");

            modelBuilder.HasSequence("seq_unidad_medida_id");

            modelBuilder.HasSequence("seq_usuario");

            modelBuilder.HasSequence("seq_usuario_perfil");

            modelBuilder.HasSequence("seq_venta_paquete");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
