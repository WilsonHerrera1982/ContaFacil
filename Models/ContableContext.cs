using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ContaFacil.Models;

public partial class ContableContext : DbContext
{
    public ContableContext()
    {
    }

    public ContableContext(DbContextOptions<ContableContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anticipo> Anticipos { get; set; }

    public virtual DbSet<AnticipoCuentaPagar> AnticipoCuentaPagars { get; set; }

    public virtual DbSet<AnticipoCuentum> AnticipoCuenta { get; set; }

    public virtual DbSet<AnticiposProveedor> AnticiposProveedors { get; set; }

    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Comision> Comisions { get; set; }

    public virtual DbSet<ComisionContador> ComisionContadors { get; set; }

    public virtual DbSet<ConstatacionFisica> ConstatacionFisicas { get; set; }

    public virtual DbSet<CuentaCobrar> CuentaCobrars { get; set; }

    public virtual DbSet<CuentasPorPagar> CuentasPorPagars { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Despacho> Despachos { get; set; }

    public virtual DbSet<DetalleDespacho> DetalleDespachos { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<Emisor> Emisors { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<GuiaRemision> GuiaRemisions { get; set; }

    public virtual DbSet<GuiaRemisionDetalle> GuiaRemisionDetalles { get; set; }

    public virtual DbSet<HistoricoProducto> HistoricoProductos { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuPerfil> MenuPerfils { get; set; }

    public virtual DbSet<NotaCredito> NotaCreditos { get; set; }

    public virtual DbSet<OpcionCliente> OpcionClientes { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Paquete> Paquetes { get; set; }

    public virtual DbSet<PaqueteContador> PaqueteContadors { get; set; }

    public virtual DbSet<Parametro> Parametros { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoProveedor> ProductoProveedors { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Retencion> Retencions { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<SucursalFactura> SucursalFacturas { get; set; }

    public virtual DbSet<SucursalInventario> SucursalInventarios { get; set; }

    public virtual DbSet<TipoIdentificacion> TipoIdentificacions { get; set; }

    public virtual DbSet<TipoPago> TipoPagos { get; set; }

    public virtual DbSet<TipoTransaccion> TipoTransaccions { get; set; }

    public virtual DbSet<Tipocuentum> Tipocuenta { get; set; }

    public virtual DbSet<Transaccion> Transaccions { get; set; }

    public virtual DbSet<Transportistum> Transportista { get; set; }

    public virtual DbSet<UnidadMedidum> UnidadMedida { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioPerfil> UsuarioPerfils { get; set; }

    public virtual DbSet<UsuarioSucursal> UsuarioSucursals { get; set; }

    public virtual DbSet<VentaPaquete> VentaPaquetes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
      => optionsBuilder.UseMySql("Server=localhost;Database=contable;User=root;Password=ROOT;ConvertZeroDateTime=True;", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Anticipo>(entity =>
        {
            entity.HasKey(e => e.IdAnticipo).HasName("PRIMARY");

            entity.ToTable("anticipo", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdCliente, "anticipo_id_cliente_fkey");

            entity.HasIndex(e => e.IdEmpresa, "anticipo_id_empresa_fkey");

            entity.Property(e => e.IdAnticipo)
                .HasComment("TRIAL")
                .HasColumnName("id_anticipo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCheque)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_cheque");
            entity.Property(e => e.FechaComprobante)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_comprobante");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCliente)
                .HasComment("TRIAL")
                .HasColumnName("id_cliente");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.NumeroCheque)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_cheque");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.PagueseOrden)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("paguese_orden");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("tipo_pago");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Anticipos)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("anticipo_id_cliente_fkey");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Anticipos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("anticipo_id_empresa_fkey");
        });

        modelBuilder.Entity<AnticipoCuentaPagar>(entity =>
        {
            entity.HasKey(e => e.IdRelacion).HasName("PRIMARY");

            entity
                .ToTable("anticipo_cuenta_pagar")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdAnticipo, "idx_anticipo");

            entity.HasIndex(e => e.IdCuentaPorPagar, "idx_cuenta_por_pagar");

            entity.HasIndex(e => new { e.IdAnticipo, e.IdCuentaPorPagar }, "uk_anticipo_cuenta").IsUnique();

            entity.Property(e => e.IdRelacion).HasColumnName("id_relacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdAnticipo).HasColumnName("id_anticipo");
            entity.Property(e => e.IdCuentaPorPagar).HasColumnName("id_cuenta_por_pagar");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");

            entity.HasOne(d => d.IdAnticipoNavigation).WithMany(p => p.AnticipoCuentaPagars)
                .HasForeignKey(d => d.IdAnticipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_anticipo");

            entity.HasOne(d => d.IdCuentaPorPagarNavigation).WithMany(p => p.AnticipoCuentaPagars)
                .HasForeignKey(d => d.IdCuentaPorPagar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cuenta_por_pagar");
        });

        modelBuilder.Entity<AnticipoCuentum>(entity =>
        {
            entity.HasKey(e => e.IdAnticipoCuenta).HasName("PRIMARY");

            entity.ToTable("anticipo_cuenta", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdAnticipo, "anticipo_cuenta_id_anticipo_fkey");

            entity.HasIndex(e => e.IdCuenta, "fki_cuenta_porcobrar_anticipo");

            entity.Property(e => e.IdAnticipoCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_anticipo_cuenta");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdAnticipo)
                .HasComment("TRIAL")
                .HasColumnName("id_anticipo");
            entity.Property(e => e.IdCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_cuenta");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdAnticipoNavigation).WithMany(p => p.AnticipoCuenta)
                .HasForeignKey(d => d.IdAnticipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("anticipo_cuenta_id_anticipo_fkey");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.AnticipoCuenta)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_porcobrar_anticipo");
        });

        modelBuilder.Entity<AnticiposProveedor>(entity =>
        {
            entity.HasKey(e => e.IdAnticipo).HasName("PRIMARY");

            entity
                .ToTable("anticipos_proveedor")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.IdProveedor, "fk_anticipo_proveedor");

            entity.HasIndex(e => e.Estado, "idx_estado");

            entity.HasIndex(e => e.FechaAnticipo, "idx_fecha_anticipo");

            entity.Property(e => e.IdAnticipo).HasColumnName("id_anticipo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'VIGENTE'")
                .HasColumnType("enum('VIGENTE','APLICADO','ANULADO')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAnticipo).HasColumnName("fecha_anticipo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FormaPago)
                .HasColumnType("enum('EFECTIVO','TRANSFERENCIA','CHEQUE','OTRO')")
                .HasColumnName("forma_pago");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.MontoAnticipo)
                .HasPrecision(12, 2)
                .HasColumnName("monto_anticipo");
            entity.Property(e => e.NumeroAnticipo)
                .HasMaxLength(50)
                .HasColumnName("numero_anticipo");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .HasColumnName("numero_referencia");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.AnticiposProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_anticipo_proveedor");
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaProducto).HasName("PRIMARY");

            entity.ToTable("categoria_producto", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "categoria_producto_empresa_fk");

            entity.Property(e => e.IdCategoriaProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_categoria_producto");
            entity.Property(e => e.Descripcion)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.CategoriaProductos)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("categoria_producto_empresa_fk");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PRIMARY");

            entity.ToTable("cliente", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdPersona, "cliente_id_persona_fkey");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cliente");

            entity.Property(e => e.IdCliente)
                .HasComment("TRIAL")
                .HasColumnName("id_cliente");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdPersona)
                .HasComment("TRIAL")
                .HasColumnName("id_persona");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_cliente");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cliente_id_persona_fkey");
        });

        modelBuilder.Entity<Comision>(entity =>
        {
            entity.HasKey(e => e.IdComision).HasName("PRIMARY");

            entity.ToTable("comision", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdPaquete, "comision_id_paquete_fkey");

            entity.Property(e => e.IdComision)
                .HasComment("TRIAL")
                .HasColumnName("id_comision");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete)
                .HasComment("TRIAL")
                .HasColumnName("id_paquete");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.Comisions)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comision_id_paquete_fkey");
        });

        modelBuilder.Entity<ComisionContador>(entity =>
        {
            entity.HasKey(e => e.IdComsionContador).HasName("PRIMARY");

            entity.ToTable("comision_contador", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdComision, "comision_contador_id_comision_fkey");

            entity.Property(e => e.IdComsionContador)
                .HasComment("TRIAL")
                .HasColumnName("id_comsion_contador");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdComision)
                .HasComment("TRIAL")
                .HasColumnName("id_comision");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdComisionNavigation).WithMany(p => p.ComisionContadors)
                .HasForeignKey(d => d.IdComision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comision_contador_id_comision_fkey");
        });

        modelBuilder.Entity<ConstatacionFisica>(entity =>
        {
            entity.HasKey(e => e.ConstatacionId).HasName("PRIMARY");

            entity.ToTable("constatacion_fisica", tb => tb.HasComment("tabla para el registro de constatacion fisica de productos por empresa"));

            entity.HasIndex(e => e.EmpresaId, "FK_empresa_constatacion");

            entity.HasIndex(e => e.ProductoId, "FK_producto_constatacion");

            entity.Property(e => e.ConstatacionId).HasColumnName("constatacion_id");
            entity.Property(e => e.CantidadFisica).HasColumnName("cantidad_fisica");
            entity.Property(e => e.ConstatacionDescripcion)
                .HasMaxLength(200)
                .HasDefaultValueSql("'0'")
                .HasColumnName("constatacion_descripcion");
            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'1'")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.ProductoId).HasColumnName("producto_id");
            entity.Property(e => e.ProductoStockActual).HasColumnName("producto_stock_actual");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.Empresa).WithMany(p => p.ConstatacionFisicas)
                .HasForeignKey(d => d.EmpresaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_empresa_constatacion");

            entity.HasOne(d => d.Producto).WithMany(p => p.ConstatacionFisicas)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_producto_constatacion");
        });

        modelBuilder.Entity<CuentaCobrar>(entity =>
        {
            entity.HasKey(e => e.IdCuentaCobrar).HasName("PRIMARY");

            entity.ToTable("cuenta_cobrar", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "cuenta_cobrar_id_empresa_fkey");

            entity.HasIndex(e => e.IdFactura, "cuenta_cobrar_id_factura_fkey");

            entity.Property(e => e.IdCuentaCobrar)
                .HasComment("TRIAL")
                .HasColumnName("id_cuenta_cobrar");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.EstadoCobro)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("estado_cobro");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.Impuesto)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("impuesto");
            entity.Property(e => e.PlazoDias)
                .HasComment("TRIAL")
                .HasColumnName("plazo_dias");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_venta");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.CuentaCobrars)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_cobrar_id_empresa_fkey");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.CuentaCobrars)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_cobrar_id_factura_fkey");
        });

        modelBuilder.Entity<CuentasPorPagar>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PRIMARY");

            entity
                .ToTable("cuentas_por_pagar")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Estado, "idx_estado");

            entity.HasIndex(e => e.FechaEmision, "idx_fecha_emision");

            entity.HasIndex(e => e.FechaVencimiento, "idx_fecha_vencimiento");

            entity.HasIndex(e => e.RazonSocial, "idx_proveedor");

            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .HasColumnName("categoria");
            entity.Property(e => e.CentroCosto)
                .HasMaxLength(100)
                .HasColumnName("centro_costo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuentos)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("descuentos");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'PENDIENTE'")
                .HasColumnType("enum('PENDIENTE','PAGADO','PARCIAL','VENCIDO','ANULADO')")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
            entity.Property(e => e.FechaModificacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("timestamp")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.FormaPago)
                .HasColumnType("enum('EFECTIVO','TRANSFERENCIA','CHEQUE','TARJETA','OTRO')")
                .HasColumnName("forma_pago");
            entity.Property(e => e.Impuestos)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("impuestos");
            entity.Property(e => e.MontoPendiente)
                .HasPrecision(12, 2)
                .HasColumnName("monto_pendiente");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(12, 2)
                .HasColumnName("monto_total");
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(50)
                .HasColumnName("numero_documento");
            entity.Property(e => e.NumeroReferencia)
                .HasMaxLength(100)
                .HasColumnName("numero_referencia");
            entity.Property(e => e.Proyecto)
                .HasMaxLength(100)
                .HasColumnName("proyecto");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(200)
                .HasColumnName("razon_social");
            entity.Property(e => e.RucIdentificacion)
                .HasMaxLength(20)
                .HasColumnName("ruc_identificacion");
            entity.Property(e => e.TipoDocumento)
                .HasColumnType("enum('FACTURA','RECIBO','NOTA_CREDITO','SERVICIO','OTRO')")
                .HasColumnName("tipo_documento");
            entity.Property(e => e.TipoProveedor)
                .HasColumnType("enum('PROVEEDOR','SERVICIO_PUBLICO','SERVICIO_PRIVADO','OTROS')")
                .HasColumnName("tipo_proveedor");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PRIMARY");

            entity.ToTable("cuenta", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdTipoCuenta, "cuenta_id_tipo_cuenta_fkey");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cuenta");

            entity.Property(e => e.IdCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_cuenta");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("codigo");
            entity.Property(e => e.Credito)
                .HasComment("TRIAL")
                .HasColumnName("credito");
            entity.Property(e => e.Debito)
                .HasComment("TRIAL")
                .HasColumnName("debito");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdIdCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_id_cuenta");
            entity.Property(e => e.IdTipoCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_cuenta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.SaldoActual)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("saldo_actual");
            entity.Property(e => e.SaldoInicial)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("saldo_inicial");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_cuenta");

            entity.HasOne(d => d.IdTipoCuentaNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdTipoCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_id_tipo_cuenta_fkey");
        });

        modelBuilder.Entity<Despacho>(entity =>
        {
            entity.HasKey(e => e.IdDespacho).HasName("PRIMARY");

            entity.ToTable("despacho", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "despacho_id_empresa_fkey");

            entity.HasIndex(e => e.IdSucursal, "despacho_id_sucursal_fkey");

            entity.HasIndex(e => e.IdUsuario, "despacho_id_usuario_fkey");

            entity.Property(e => e.IdDespacho)
                .HasComment("TRIAL")
                .HasColumnName("id_despacho");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.EstadoDespacho)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("estado_despacho");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalDestino)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal_destino");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("numero_despacho");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Despachos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("despacho_id_empresa_fkey");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Despachos)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("despacho_id_sucursal_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Despachos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("despacho_id_usuario_fkey");
        });

        modelBuilder.Entity<DetalleDespacho>(entity =>
        {
            entity.HasKey(e => e.IdDetalleDespacho).HasName("PRIMARY");

            entity.ToTable("detalle_despacho", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdProducto, "detalle_despacho_id_producto_fkey");

            entity.HasIndex(e => e.IdUsuario, "detalle_despacho_id_usuario_fkey");

            entity.HasIndex(e => e.IdDespacho, "fki_despacho_detalle_fk");

            entity.Property(e => e.IdDetalleDespacho)
                .HasComment("TRIAL")
                .HasColumnName("id_detalle_despacho");
            entity.Property(e => e.Cantidad)
                .HasComment("TRIAL")
                .HasColumnName("cantidad");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdDespacho)
                .HasComment("TRIAL")
                .HasColumnName("id_despacho");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdDespachoNavigation).WithMany(p => p.DetalleDespachos)
                .HasForeignKey(d => d.IdDespacho)
                .HasConstraintName("despacho_detalle_fk");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleDespachos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_despacho_id_producto_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DetalleDespachos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_despacho_id_usuario_fkey");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => e.IdDetalleFactura).HasName("PRIMARY");

            entity.ToTable("detalle_factura", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdFactura, "detalle_factura_id_factura_fkey");

            entity.HasIndex(e => e.IdImpuesto, "fki_fk_impuesto_detalle");

            entity.HasIndex(e => e.IdProducto, "fki_p");

            entity.Property(e => e.IdDetalleFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_detalle_factura");
            entity.Property(e => e.Cantidad)
                .HasComment("TRIAL")
                .HasColumnName("cantidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("descuento");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.IdImpuesto)
                .HasComment("TRIAL")
                .HasColumnName("id_impuesto");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_factura_id_factura_fkey");

            entity.HasOne(d => d.IdImpuestoNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.IdImpuesto)
                .HasConstraintName("fk_impuesto_detalle");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("detalle_producto");
        });

        modelBuilder.Entity<Emisor>(entity =>
        {
            entity.HasKey(e => e.IdEmisor).HasName("PRIMARY");

            entity.ToTable("emisor", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "emisor_id_empresa_fkey");

            entity.HasIndex(e => e.IdUsuario, "emisor_id_usuario_fkey");

            entity.Property(e => e.IdEmisor)
                .HasComment("TRIAL")
                .HasColumnName("id_emisor");
            entity.Property(e => e.CertificadoDigital)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("certificado_digital");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("clave");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("correo_electronico");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("direccion");
            entity.Property(e => e.Establecimiento)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("establecimiento");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.NombreComercial)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre_comercial");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.ObligadoContabilidad)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("obligado_contabilidad");
            entity.Property(e => e.PuntoEmision)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("punto_emision");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("razon_social");
            entity.Property(e => e.Ruc)
                .HasMaxLength(13)
                .HasComment("TRIAL")
                .HasColumnName("ruc");
            entity.Property(e => e.Secuencial)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("secuencial");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.TipoAmbiente)
                .HasMaxLength(1)
                .HasComment("TRIAL")
                .HasColumnName("tipo_ambiente");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Emisors)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("emisor_id_empresa_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Emisors)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("emisor_id_usuario_fkey");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PRIMARY");

            entity.ToTable("empresa", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PRIMARY");

            entity.ToTable("factura", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdCliente, "factura_id_cliente_fkey");

            entity.HasIndex(e => e.IdEmisor, "fki_fk_emisor_factura");

            entity.HasIndex(e => e.IdSucursal, "sucursal_factura_fk");

            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.AutorizacionSri)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("autorizacion_sri");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("clave_acceso");
            entity.Property(e => e.Credito)
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("credito");
            entity.Property(e => e.DescripcionSri)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("descripcion_sri");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.Fecha)
                .HasComment("TRIAL")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaAutorizacionSri)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_autorizacion_sri");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCliente)
                .HasComment("TRIAL")
                .HasColumnName("id_cliente");
            entity.Property(e => e.IdEmisor)
                .HasComment("TRIAL")
                .HasColumnName("id_emisor");
            entity.Property(e => e.IdImpuesto)
                .HasComment("TRIAL")
                .HasColumnName("id_impuesto");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("monto_total");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("numero_factura");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("subtotal");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Xml)
                .HasMaxLength(15000)
                .HasComment("TRIAL")
                .HasColumnName("xml");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("factura_id_cliente_fkey");

            entity.HasOne(d => d.IdEmisorNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.IdEmisor)
                .HasConstraintName("fk_emisor_factura");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("sucursal_factura_fk");
        });

        modelBuilder.Entity<GuiaRemision>(entity =>
        {
            entity.HasKey(e => e.IdGuiaRemision).HasName("PRIMARY");

            entity.ToTable("guia_remision", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.FechaEmision, "idx_guia_remision_fecha");

            entity.HasIndex(e => e.NumeroGuia, "idx_guia_remision_numero");

            entity.HasIndex(e => e.IdSucursal, "sucursal_guia_remision_fk");

            entity.HasIndex(e => e.IdTransportista, "transportista_guia_remision_fk");

            entity.Property(e => e.IdGuiaRemision)
                .HasComment("TRIAL")
                .HasColumnName("id_guia_remision");
            entity.Property(e => e.AmbienteSri)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("ambiente_sri");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("clave_acceso");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.EstadoSri)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("estado_sri");
            entity.Property(e => e.FechaAutorizacionSri)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_autorizacion_sri");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaFinTraslado)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin_traslado");
            entity.Property(e => e.FechaInicioTraslado)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio_traslado");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.IdTransportista)
                .HasComment("TRIAL")
                .HasColumnName("id_transportista");
            entity.Property(e => e.MotivoTraslado)
                .HasMaxLength(300)
                .HasComment("TRIAL")
                .HasColumnName("motivo_traslado");
            entity.Property(e => e.NumeroAutorizacionSri)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("numero_autorizacion_sri");
            entity.Property(e => e.NumeroGuia)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("numero_guia");
            entity.Property(e => e.PlacaVehiculo)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("placa_vehiculo");
            entity.Property(e => e.PuntoLlegada)
                .HasComment("TRIAL")
                .HasColumnName("punto_llegada");
            entity.Property(e => e.PuntoPartida)
                .HasComment("TRIAL")
                .HasColumnName("punto_partida");
            entity.Property(e => e.RespuestaSri)
                .HasComment("TRIAL")
                .HasColumnName("respuesta_sri");
            entity.Property(e => e.Ruta)
                .HasComment("TRIAL")
                .HasColumnName("ruta");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.XmlSri)
                .HasComment("TRIAL")
                .HasColumnName("xml_sri");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.GuiaRemisions)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("sucursal_guia_remision_fk");

            entity.HasOne(d => d.IdTransportistaNavigation).WithMany(p => p.GuiaRemisions)
                .HasForeignKey(d => d.IdTransportista)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transportista_guia_remision_fk");
        });

        modelBuilder.Entity<GuiaRemisionDetalle>(entity =>
        {
            entity.HasKey(e => e.IdGuiaRemisionDetalle).HasName("PRIMARY");

            entity.ToTable("guia_remision_detalle", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdGuiaRemision, "guia_remision_detalle_id_guia_remision_fkey");

            entity.HasIndex(e => e.IdProducto, "producto_guia_remision_detalle_fk");

            entity.Property(e => e.IdGuiaRemisionDetalle)
                .HasComment("TRIAL")
                .HasColumnName("id_guia_remision_detalle");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("cantidad");
            entity.Property(e => e.CodigoAuxiliar)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("codigo_auxiliar");
            entity.Property(e => e.CodigoPrincipal)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("codigo_principal");
            entity.Property(e => e.Descripcion)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdGuiaRemision)
                .HasComment("TRIAL")
                .HasColumnName("id_guia_remision");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("unidad_medida");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdGuiaRemisionNavigation).WithMany(p => p.GuiaRemisionDetalles)
                .HasForeignKey(d => d.IdGuiaRemision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("guia_remision_detalle_id_guia_remision_fkey");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.GuiaRemisionDetalles)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("producto_guia_remision_detalle_fk");
        });

        modelBuilder.Entity<HistoricoProducto>(entity =>
        {
            entity.HasKey(e => e.IdHistoricoProducto).HasName("PRIMARY");

            entity.ToTable("historico_producto", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "historico_producto_id_empresa_fkey");

            entity.HasIndex(e => e.IdProducto, "historico_producto_id_producto_fkey");

            entity.Property(e => e.IdHistoricoProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_historico_producto");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.Impuesto)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("impuesto");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_despacho");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_venta");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.HistoricoProductos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historico_producto_id_empresa_fkey");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.HistoricoProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historico_producto_id_producto_fkey");
        });

        modelBuilder.Entity<Impuesto>(entity =>
        {
            entity.HasKey(e => e.IdImpuesto).HasName("PRIMARY");

            entity.ToTable("impuesto", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdImpuesto)
                .HasComment("TRIAL")
                .HasColumnName("id_impuesto");
            entity.Property(e => e.CodigoPorcentajeSri)
                .HasComment("TRIAL")
                .HasColumnName("codigo_porcentaje_sri");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("codigo_sri");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasComment("TRIAL")
                .HasColumnName("porcentaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("tipo");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PRIMARY");

            entity.ToTable("inventario", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdProducto, "inventario_id_producto_fkey");

            entity.HasIndex(e => e.IdSucursal, "sucursal_inventario_fk");

            entity.Property(e => e.IdInventario)
                .HasComment("TRIAL")
                .HasColumnName("id_inventario");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("cantidad");
            entity.Property(e => e.Descripcion)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("descuento");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FacturaNumero)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("factura_numero");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdCuentaContable)
                .HasComment("TRIAL")
                .HasColumnName("id_cuenta_contable");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.Iva)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("iva");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("numero_despacho");
            entity.Property(e => e.PrecioCalculo)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_calculo");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.Stock)
                .HasComment("TRIAL")
                .HasColumnName("stock");
            entity.Property(e => e.SubTotal)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("sub_total");
            entity.Property(e => e.Subtotal15)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("subtotal15");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("total");
            entity.Property(e => e.TransaccionRegistrada)
                .HasDefaultValueSql("'0'")
                .HasComment("TRIAL")
                .HasColumnName("transaccion_registrada");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("inventario_id_producto_fkey");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("sucursal_inventario_fk");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("PRIMARY");

            entity.ToTable("menu", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdMenu)
                .HasComment("TRIAL")
                .HasColumnName("id_menu");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("action");
            entity.Property(e => e.Controller)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("controller");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.MenuId)
                .HasComment("TRIAL")
                .HasColumnName("menu_id");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Url)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("url");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<MenuPerfil>(entity =>
        {
            entity.HasKey(e => e.IdMenuPerfil).HasName("PRIMARY");

            entity.ToTable("menu_perfil", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdMenu, "menu_perfil_id_menu_fkey");

            entity.HasIndex(e => e.IdPerfil, "menu_perfil_id_perfil_fkey");

            entity.Property(e => e.IdMenuPerfil)
                .HasComment("TRIAL")
                .HasColumnName("id_menu_perfil");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdMenu)
                .HasComment("TRIAL")
                .HasColumnName("id_menu");
            entity.Property(e => e.IdPerfil)
                .HasComment("TRIAL")
                .HasColumnName("id_perfil");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.MenuPerfils)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_perfil_id_menu_fkey");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.MenuPerfils)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("menu_perfil_id_perfil_fkey");
        });

        modelBuilder.Entity<NotaCredito>(entity =>
        {
            entity.HasKey(e => e.IdNotaCredito).HasName("PRIMARY");

            entity.ToTable("nota_credito", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "nota_credito_id_empresa_fkey");

            entity.HasIndex(e => e.IdFactura, "nota_credito_id_factura_fkey");

            entity.Property(e => e.IdNotaCredito)
                .HasComment("TRIAL")
                .HasColumnName("id_nota_credito");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("clave_acceso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaAutorizacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_autorizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.Motivo)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("motivo");
            entity.Property(e => e.NumeroAutorizacion)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_autorizacion");
            entity.Property(e => e.NumeroNota)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_nota");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Xml)
                .HasMaxLength(15000)
                .HasComment("TRIAL")
                .HasColumnName("xml");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.NotaCreditos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("nota_credito_id_empresa_fkey");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.NotaCreditos)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("nota_credito_id_factura_fkey");
        });

        modelBuilder.Entity<OpcionCliente>(entity =>
        {
            entity.HasKey(e => e.OpcionId).HasName("PRIMARY");

            entity.ToTable("opcion_cliente", tb => tb.HasComment("tabla para registrar el acceso a opciones de cliente"));

            entity.HasIndex(e => e.EmpresaId, "FK_empresa_opcion");

            entity.Property(e => e.OpcionId).HasColumnName("opcion_id");
            entity.Property(e => e.EmpresaId).HasColumnName("empresa_id");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaResgitro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_resgitro");
            entity.Property(e => e.OpcionDescripcion)
                .HasMaxLength(200)
                .HasColumnName("opcion_descripcion");
            entity.Property(e => e.OpcionNombre)
                .HasMaxLength(100)
                .HasColumnName("opcion_nombre");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.Empresa).WithMany(p => p.OpcionClientes)
                .HasForeignKey(d => d.EmpresaId)
                .HasConstraintName("FK_empresa_opcion");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PRIMARY");

            entity.ToTable("pago", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdFactura, "pago_id_factura_fkey");

            entity.HasIndex(e => e.IdTipoPago, "pago_id_tipo_pago_fkey");

            entity.Property(e => e.IdPago)
                .HasComment("TRIAL")
                .HasColumnName("id_pago");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasComment("TRIAL")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.IdTipoPago)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_pago");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("monto");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_id_factura_fkey");

            entity.HasOne(d => d.IdTipoPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdTipoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_id_tipo_pago_fkey");
        });

        modelBuilder.Entity<Paquete>(entity =>
        {
            entity.HasKey(e => e.IdPaquete).HasName("PRIMARY");

            entity.ToTable("paquete", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdPaquete)
                .HasComment("TRIAL")
                .HasColumnName("id_paquete");
            entity.Property(e => e.CantidadEmisores)
                .HasComment("TRIAL")
                .HasColumnName("cantidad_emisores");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor");
        });

        modelBuilder.Entity<PaqueteContador>(entity =>
        {
            entity.HasKey(e => e.IdPaqueteContador).HasName("PRIMARY");

            entity.ToTable("paquete_contador", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdPaquete, "paquete_contador_id_paquete_fkey");

            entity.HasIndex(e => e.IdUsuario, "paquete_contador_id_usuario_fkey");

            entity.Property(e => e.IdPaqueteContador)
                .HasComment("TRIAL")
                .HasColumnName("id_paquete_contador");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete)
                .HasComment("TRIAL")
                .HasColumnName("id_paquete");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.PaqueteContadors)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("paquete_contador_id_paquete_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PaqueteContadors)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("paquete_contador_id_usuario_fkey");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.HasKey(e => e.IdParametro).HasName("PRIMARY");

            entity.ToTable("parametro", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "parametro_id_empresa_fkey");

            entity.Property(e => e.IdParametro)
                .HasComment("TRIAL")
                .HasColumnName("id_parametro");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.NombreParametro)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre_parametro");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("valor");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Parametros)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parametro_id_empresa_fkey");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.IdPerfil).HasName("PRIMARY");

            entity.ToTable("perfil", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdPerfil)
                .HasComment("TRIAL")
                .HasColumnName("id_perfil");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PRIMARY");

            entity.ToTable("persona", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_persona");

            entity.HasIndex(e => e.IdTipoIdentificacion, "fki_tipo_identiificacion_persona_fk");

            entity.Property(e => e.IdPersona)
                .HasComment("TRIAL")
                .HasColumnName("id_persona");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasComment("TRIAL")
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdTipoIdentificacion)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_identificacion");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(13)
                .HasComment("TRIAL")
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.RetencionIva)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("'0.00'")
                .HasComment("TRIAL")
                .HasColumnName("retencion_iva");
            entity.Property(e => e.RetencionPorcentaje)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("'0.00'")
                .HasComment("TRIAL")
                .HasColumnName("retencion_porcentaje");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_persona");

            entity.HasOne(d => d.IdTipoIdentificacionNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdTipoIdentificacion)
                .HasConstraintName("tipo_identiificacion_persona_fk");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PRIMARY");

            entity.ToTable("producto", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "fki_fk_producto_empresa");

            entity.HasIndex(e => e.IdImpuesto, "fki_i");

            entity.HasIndex(e => e.Codigo, "producto_codigo_key").IsUnique();

            entity.HasIndex(e => e.IdCategoriaProducto, "producto_id_categoria_producto_fkey");

            entity.HasIndex(e => e.IdUnidadMedida, "producto_id_unidad_medida_fkey");

            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.CantidadMinima)
                .HasComment("cantidad minima de porductos")
                .HasColumnName("cantidad_minima");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("descuento%");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCategoriaProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_categoria_producto");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdImpuesto)
                .HasComment("TRIAL")
                .HasColumnName("id_impuesto");
            entity.Property(e => e.IdUnidadMedida)
                .HasComment("TRIAL")
                .HasColumnName("id_unidad_medida");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_venta");
            entity.Property(e => e.Stock)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasComment("TRIAL")
                .HasColumnName("stock");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.Utilidad)
                .HasComment("TRIAL")
                .HasColumnName("utilidad%");

            entity.HasOne(d => d.IdCategoriaProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoriaProducto)
                .HasConstraintName("producto_id_categoria_producto_fkey");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("fk_producto_empresa");

            entity.HasOne(d => d.IdImpuestoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdImpuesto)
                .HasConstraintName("fk_producto_impuesto");

            entity.HasOne(d => d.IdUnidadMedidaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdUnidadMedida)
                .HasConstraintName("producto_id_unidad_medida_fkey");
        });

        modelBuilder.Entity<ProductoProveedor>(entity =>
        {
            entity.HasKey(e => e.IdProductoProveedor).HasName("PRIMARY");

            entity.ToTable("producto_proveedor", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdProducto, "producto_proveedor_id_producto_fkey");

            entity.Property(e => e.IdProductoProveedor)
                .HasComment("TRIAL")
                .HasColumnName("id_producto_proveedor");
            entity.Property(e => e.Cantidad)
                .HasComment("TRIAL")
                .HasColumnName("cantidad");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdProducto)
                .HasComment("TRIAL")
                .HasColumnName("id_producto");
            entity.Property(e => e.IdProveedor)
                .HasComment("TRIAL")
                .HasColumnName("id_proveedor");
            entity.Property(e => e.PrecioCompra)
                .HasPrecision(10, 2)
                .HasComment("TRIAL")
                .HasColumnName("precio_compra");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("producto_proveedor_id_producto_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PRIMARY");

            entity.ToTable("proveedor", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_proveedor");

            entity.Property(e => e.IdProveedor)
                .HasComment("TRIAL")
                .HasColumnName("id_proveedor");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasComment("TRIAL")
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(15)
                .HasComment("TRIAL")
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.RetencionIva)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("retencion_iva");
            entity.Property(e => e.RetencionPorcentaje)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("retencion_porcentaje");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Proveedors)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_proveedor");
        });

        modelBuilder.Entity<Retencion>(entity =>
        {
            entity.HasKey(e => e.IdRetencion).HasName("PRIMARY");

            entity.ToTable("retencion", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "retencion_id_empresa_fkey");

            entity.Property(e => e.IdRetencion)
                .HasComment("TRIAL")
                .HasColumnName("id_retencion");
            entity.Property(e => e.BaseImponible)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("base_imponible");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("clave_acceso");
            entity.Property(e => e.ComprobanteRetencion)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("comprobante_retencion");
            entity.Property(e => e.EjercicioFiscal)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("ejercicio_fiscal");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaAutorizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_autorizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.IdProveedor)
                .HasComment("TRIAL")
                .HasColumnName("id_proveedor");
            entity.Property(e => e.Impuesto)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("impuesto");
            entity.Property(e => e.NumeroAutorizacion)
                .HasMaxLength(200)
                .HasComment("TRIAL")
                .HasColumnName("numero_autorizacion");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("numero_factura");
            entity.Property(e => e.PorcentajeRetencion)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("porcentaje_retencion");
            entity.Property(e => e.Proveedor)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("proveedor");
            entity.Property(e => e.TipoContribuyente)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("tipo_contribuyente");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.ValorRetenido)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("valor_retenido");
            entity.Property(e => e.Xml)
                .HasMaxLength(15000)
                .HasComment("TRIAL")
                .HasColumnName("xml");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Retencions)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("retencion_id_empresa_fkey");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PRIMARY");

            entity.ToTable("sucursal", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmisor, "sucursal_id_emisor_fkey");

            entity.HasIndex(e => e.IdUsuario, "sucursal_id_usuario_fkey");

            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("clave");
            entity.Property(e => e.DireccionSucursal)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("direccion_sucursal");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmisor)
                .HasComment("TRIAL")
                .HasColumnName("id_emisor");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.NombreSucursal)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre_sucursal");
            entity.Property(e => e.PuntoEmision)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("punto_emision");
            entity.Property(e => e.Secuencial)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("secuencial");
            entity.Property(e => e.Telefono)
                .HasMaxLength(13)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("usuario");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmisorNavigation).WithMany(p => p.Sucursals)
                .HasForeignKey(d => d.IdEmisor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_id_emisor_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Sucursals)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_id_usuario_fkey");
        });

        modelBuilder.Entity<SucursalFactura>(entity =>
        {
            entity.HasKey(e => e.IdSucursalFactura).HasName("PRIMARY");

            entity.ToTable("sucursal_factura", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdFactura, "sucursal_factura_id_factura_fkey");

            entity.HasIndex(e => e.IdSucursal, "sucursal_factura_id_sucursal_fkey");

            entity.Property(e => e.IdSucursalFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal_factura");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura)
                .HasComment("TRIAL")
                .HasColumnName("id_factura");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.SucursalFacturas)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_factura_id_factura_fkey");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.SucursalFacturas)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_factura_id_sucursal_fkey");
        });

        modelBuilder.Entity<SucursalInventario>(entity =>
        {
            entity.HasKey(e => e.IdSucursalInventario).HasName("PRIMARY");

            entity.ToTable("sucursal_inventario", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdInventario, "sucursal_inventario_id_inventario_fkey");

            entity.HasIndex(e => e.IdSucursal, "sucursal_inventario_id_sucursal_fkey");

            entity.Property(e => e.IdSucursalInventario)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal_inventario");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdInventario)
                .HasComment("TRIAL")
                .HasColumnName("id_inventario");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.SucursalInventarios)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_inventario_id_inventario_fkey");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.SucursalInventarios)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sucursal_inventario_id_sucursal_fkey");
        });

        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoIdemtificacion).HasName("PRIMARY");

            entity.ToTable("tipo_identificacion", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdTipoIdemtificacion)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_idemtificacion");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("codigo_sri");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<TipoPago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("PRIMARY");

            entity.ToTable("tipo_pago", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdTipoPago)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_pago");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("codigo_sri");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
        });

        modelBuilder.Entity<TipoTransaccion>(entity =>
        {
            entity.HasKey(e => e.IdTipoTransaccion).HasName("PRIMARY");

            entity.ToTable("tipo_transaccion", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdTipoTransaccion)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_transaccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
        });

        modelBuilder.Entity<Tipocuentum>(entity =>
        {
            entity.HasKey(e => e.IdTipoCuenta).HasName("PRIMARY");

            entity.ToTable("tipocuenta", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdTipoCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_cuenta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("PRIMARY");

            entity.ToTable("transaccion", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_transaccion");

            entity.HasIndex(e => e.IdCuenta, "transaccion_id_cuenta_fkey");

            entity.HasIndex(e => e.IdTipoTransaccion, "transaccion_id_tipo_transaccion_fkey");

            entity.Property(e => e.IdTransaccion)
                .HasComment("TRIAL")
                .HasColumnName("id_transaccion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasComment("TRIAL")
                .HasColumnName("descripcion");
            entity.Property(e => e.EsDebito)
                .HasDefaultValueSql("'0'")
                .HasComment("TRIAL")
                .HasColumnName("esdebito");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasComment("TRIAL")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCuenta)
                .HasComment("TRIAL")
                .HasColumnName("id_cuenta");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdInventario)
                .HasComment("TRIAL")
                .HasColumnName("id_inventario");
            entity.Property(e => e.IdTipoTransaccion)
                .HasComment("TRIAL")
                .HasColumnName("id_tipo_transaccion");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasComment("TRIAL")
                .HasColumnName("monto");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transaccion_id_cuenta_fkey");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_transaccion");

            entity.HasOne(d => d.IdTipoTransaccionNavigation).WithMany(p => p.Transaccions)
                .HasForeignKey(d => d.IdTipoTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transaccion_id_tipo_transaccion_fkey");
        });

        modelBuilder.Entity<Transportistum>(entity =>
        {
            entity.HasKey(e => e.IdTransportista).HasName("PRIMARY");

            entity.ToTable("transportista", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => new { e.TipoIdentificacion, e.NumeroIdentificacion }, "uk_transportista_identificacion").IsUnique();

            entity.Property(e => e.IdTransportista)
                .HasComment("TRIAL")
                .HasColumnName("id_transportista");
            entity.Property(e => e.Direccion)
                .HasComment("TRIAL")
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.NumeroIdentificacion)
                .HasMaxLength(13)
                .HasComment("TRIAL")
                .HasColumnName("numero_identificacion");
            entity.Property(e => e.PlacaVehiculo)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("placa_vehiculo");
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(300)
                .HasComment("TRIAL")
                .HasColumnName("razon_social");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("telefono");
            entity.Property(e => e.TipoIdentificacion)
                .HasMaxLength(20)
                .HasComment("TRIAL")
                .HasColumnName("tipo_identificacion");
            entity.Property(e => e.Trial489)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial489");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
            entity.Property(e => e.UsuarioRegistro)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("usuario_registro");
        });

        modelBuilder.Entity<UnidadMedidum>(entity =>
        {
            entity.HasKey(e => e.IdUnidadMedida).HasName("PRIMARY");

            entity.ToTable("unidad_medida", tb => tb.HasComment("TRIAL"));

            entity.Property(e => e.IdUnidadMedida)
                .HasComment("TRIAL")
                .HasColumnName("id_unidad_medida");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(10)
                .HasComment("TRIAL")
                .HasColumnName("abreviatura");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial485)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial485");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_usuario");

            entity.HasIndex(e => e.IdPersona, "usuario_id_persona_fkey");

            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("clave");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa)
                .HasComment("TRIAL")
                .HasColumnName("id_empresa");
            entity.Property(e => e.IdPersona)
                .HasComment("TRIAL")
                .HasColumnName("id_persona");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasComment("TRIAL")
                .HasColumnName("nombre");
            entity.Property(e => e.Trial482)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial482");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_usuario");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_id_persona_fkey");
        });

        modelBuilder.Entity<UsuarioPerfil>(entity =>
        {
            entity.HasKey(e => e.IdUsuarioPerfil).HasName("PRIMARY");

            entity.ToTable("usuario_perfil", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdPerfil, "usuario_perfil_id_perfil_fkey");

            entity.HasIndex(e => e.IdUsuario, "usuario_perfil_id_usuario_fkey");

            entity.Property(e => e.IdUsuarioPerfil)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario_perfil");
            entity.Property(e => e.Estado)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPerfil)
                .HasComment("TRIAL")
                .HasColumnName("id_perfil");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.UsuarioPerfils)
                .HasForeignKey(d => d.IdPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_perfil_id_perfil_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioPerfils)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_perfil_id_usuario_fkey");
        });

        modelBuilder.Entity<UsuarioSucursal>(entity =>
        {
            entity.HasKey(e => e.IdUsuarioSucursal).HasName("PRIMARY");

            entity.ToTable("usuario_sucursal", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdSucursal, "usuario_sucursal_id_sucursal_fkey");

            entity.HasIndex(e => e.IdUsuario, "usuario_sucursal_id_usuario_fkey");

            entity.Property(e => e.IdUsuarioSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario_sucursal");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdSucursal)
                .HasComment("TRIAL")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.UsuarioSucursals)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_sucursal_id_sucursal_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioSucursals)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_sucursal_id_usuario_fkey");
        });

        modelBuilder.Entity<VentaPaquete>(entity =>
        {
            entity.HasKey(e => e.IdVentaPaquete).HasName("PRIMARY");

            entity.ToTable("venta_paquete", tb => tb.HasComment("TRIAL"));

            entity.HasIndex(e => e.IdPaquete, "venta_paquete_id_paquete_fkey");

            entity.HasIndex(e => e.IdUsuario, "venta_paquete_id_usuario_fkey");

            entity.Property(e => e.IdVentaPaquete)
                .HasComment("TRIAL")
                .HasColumnName("id_venta_paquete");
            entity.Property(e => e.EstadoBoolean)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasComment("TRIAL")
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasComment("TRIAL")
                .HasColumnType("datetime")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete)
                .HasComment("TRIAL")
                .HasColumnName("id_paquete");
            entity.Property(e => e.IdUsuario)
                .HasComment("TRIAL")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Trial492)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("TRIAL")
                .HasColumnName("trial492");
            entity.Property(e => e.UsuarioCreacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion)
                .HasComment("TRIAL")
                .HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.VentaPaquetes)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venta_paquete_id_paquete_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.VentaPaquetes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venta_paquete_id_usuario_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
