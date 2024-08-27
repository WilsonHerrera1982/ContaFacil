using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<AnticipoCuentum> AnticipoCuenta { get; set; }

    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Comision> Comisions { get; set; }

    public virtual DbSet<ComisionContador> ComisionContadors { get; set; }

    public virtual DbSet<CuentaCobrar> CuentaCobrars { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Despacho> Despachos { get; set; }

    public virtual DbSet<DetalleDespacho> DetalleDespachos { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

    public virtual DbSet<Emisor> Emisors { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<HistoricoProducto> HistoricoProductos { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuPerfil> MenuPerfils { get; set; }

    public virtual DbSet<NotaCredito> NotaCreditos { get; set; }

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

    public virtual DbSet<UnidadMedidum> UnidadMedida { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioPerfil> UsuarioPerfils { get; set; }

    public virtual DbSet<UsuarioSucursal> UsuarioSucursals { get; set; }

    public virtual DbSet<VentaPaquete> VentaPaquetes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=contable;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anticipo>(entity =>
        {
            entity.HasKey(e => e.IdAnticipo).HasName("anticipo_pkey");

            entity.ToTable("anticipo");

            entity.Property(e => e.IdAnticipo)
                .HasDefaultValueSql("nextval('seq_historico_producto'::regclass)")
                .HasColumnName("id_anticipo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCheque)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_cheque");
            entity.Property(e => e.FechaComprobante)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_comprobante");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.NumeroCheque)
                .HasMaxLength(200)
                .HasColumnName("numero_cheque");
            entity.Property(e => e.NumeroComprobante)
                .HasMaxLength(200)
                .HasColumnName("numero_comprobante");
            entity.Property(e => e.PagueseOrden)
                .HasMaxLength(200)
                .HasColumnName("paguese_orden");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
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

        modelBuilder.Entity<AnticipoCuentum>(entity =>
        {
            entity.HasKey(e => e.IdAnticipoCuenta).HasName("anticipo_cuenta_pkey");

            entity.ToTable("anticipo_cuenta");

            entity.Property(e => e.IdAnticipoCuenta)
                .HasDefaultValueSql("nextval('seq_anticipo_cuenta'::regclass)")
                .HasColumnName("id_anticipo_cuenta");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdAnticipo).HasColumnName("id_anticipo");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdAnticipoNavigation).WithMany(p => p.AnticipoCuenta)
                .HasForeignKey(d => d.IdAnticipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("anticipo_cuenta_id_anticipo_fkey");
           
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaProducto).HasName("categoria_producto_pkey");

            entity.ToTable("categoria_producto");

            entity.Property(e => e.IdCategoriaProducto)
                .HasDefaultValueSql("nextval('seq_categoria_producto_id'::regclass)")
                .HasColumnName("id_categoria_producto");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.CategoriaProductos)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("categoria_producto_empresa_fk");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("cliente_pkey");

            entity.ToTable("cliente");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cliente");

            entity.Property(e => e.IdCliente)
                .HasDefaultValueSql("nextval('seq_cliente'::regclass)")
                .HasColumnName("id_cliente");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdComision).HasName("comision_pkey");

            entity.ToTable("comision");

            entity.Property(e => e.IdComision)
                .HasDefaultValueSql("nextval('seq_comision'::regclass)")
                .HasColumnName("id_comision");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.Comisions)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comision_id_paquete_fkey");
        });

        modelBuilder.Entity<ComisionContador>(entity =>
        {
            entity.HasKey(e => e.IdComsionContador).HasName("comision_contador_pkey");

            entity.ToTable("comision_contador");

            entity.Property(e => e.IdComsionContador)
                .HasDefaultValueSql("nextval('seq_comision_contador'::regclass)")
                .HasColumnName("id_comsion_contador");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdComision).HasColumnName("id_comision");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasPrecision(15, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdComisionNavigation).WithMany(p => p.ComisionContadors)
                .HasForeignKey(d => d.IdComision)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comision_contador_id_comision_fkey");
        });

        modelBuilder.Entity<CuentaCobrar>(entity =>
        {
            entity.HasKey(e => e.IdCuentaCobrar).HasName("cuenta_cobrar_pkey");

            entity.ToTable("cuenta_cobrar");

            entity.Property(e => e.IdCuentaCobrar)
                .HasDefaultValueSql("nextval('seq_cuenta_cobrar'::regclass)")
                .HasColumnName("id_cuenta_cobrar");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.EstadoCobro)
                .HasMaxLength(50)
                .HasColumnName("estado_cobro");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.Impuesto)
                .HasPrecision(15, 2)
                .HasColumnName("impuesto");
            entity.Property(e => e.PlazoDias).HasColumnName("plazo_dias");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(15, 2)
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(15, 2)
                .HasColumnName("precio_venta");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.CuentaCobrars)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_cobrar_id_empresa_fkey");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.CuentaCobrars)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cuenta_cobrar_id_factura_fkey");
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("cuenta_pkey");

            entity.ToTable("cuenta");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_cuenta");

            entity.Property(e => e.IdCuenta)
                .HasDefaultValueSql("nextval('seq_cuenta'::regclass)")
                .HasColumnName("id_cuenta");
            entity.Property(e => e.Codigo)
                .HasMaxLength(100)
                .HasColumnName("codigo");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdIdCuenta)
                .HasComment("id de la cuenta padre")
                .HasColumnName("id_id_cuenta");
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
            entity.HasKey(e => e.IdDespacho).HasName("despacho_pkey");

            entity.ToTable("despacho");

            entity.Property(e => e.IdDespacho)
                .HasDefaultValueSql("nextval('seq_despacho'::regclass)")
                .HasColumnName("id_despacho");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.EstadoDespacho)
                .HasMaxLength(50)
                .HasColumnName("estado_despacho");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdSucursalDestino).HasColumnName("id_sucursal_destino");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(100)
                .HasColumnName("numero_despacho");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdDetalleDespacho).HasName("detalle_despacho_pkey");

            entity.ToTable("detalle_despacho");

            entity.HasIndex(e => e.IdDespacho, "fki_despacho_detalle_fk");

            entity.Property(e => e.IdDetalleDespacho)
                .HasDefaultValueSql("nextval('seq_detalle_despacho'::regclass)")
                .HasColumnName("id_detalle_despacho");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdDespacho).HasColumnName("id_despacho");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdDetalleFactura).HasName("detalle_factura_pkey");

            entity.ToTable("detalle_factura");

            entity.HasIndex(e => e.IdProducto, "fki_p");

            entity.Property(e => e.IdDetalleFactura)
                .HasDefaultValueSql("nextval('seq_detalle_factura'::regclass)")
                .HasColumnName("id_detalle_factura");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasColumnName("descuento");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(15, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_factura_id_factura_fkey");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleFacturas)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("detalle_producto");
        });

        modelBuilder.Entity<Emisor>(entity =>
        {
            entity.HasKey(e => e.IdEmisor).HasName("emisor_pkey");

            entity.ToTable("emisor");

            entity.Property(e => e.IdEmisor)
                .HasDefaultValueSql("nextval('seq_emisor'::regclass)")
                .HasColumnName("id_emisor");
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
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
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
            entity.HasKey(e => e.IdEmpresa).HasName("empresa_pkey");

            entity.ToTable("empresa");

            entity.Property(e => e.IdEmpresa)
                .HasDefaultValueSql("nextval('seq_empresa'::regclass)")
                .HasColumnName("id_empresa");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
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
            entity.HasKey(e => e.IdFactura).HasName("factura_pkey");

            entity.ToTable("factura");

            entity.HasIndex(e => e.IdEmisor, "fki_fk_emisor_factura");

            entity.Property(e => e.IdFactura)
                .HasDefaultValueSql("nextval('seq_factura'::regclass)")
                .HasColumnName("id_factura");
            entity.Property(e => e.AutorizacionSri)
                .HasMaxLength(100)
                .HasColumnName("autorizacion_sri");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasColumnName("clave_acceso");
            entity.Property(e => e.Credito)
                .HasDefaultValue(true)
                .HasColumnName("credito");
            entity.Property(e => e.DescripcionSri)
                .HasMaxLength(200)
                .HasColumnName("descripcion_sri");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaAutorizacionSri)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_autorizacion_sri");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEmisor).HasColumnName("id_emisor");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.MontoTotal)
                .HasPrecision(15, 2)
                .HasColumnName("monto_total");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(100)
                .HasColumnName("numero_factura");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Xml)
                .HasMaxLength(1000000)
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

        modelBuilder.Entity<HistoricoProducto>(entity =>
        {
            entity.HasKey(e => e.IdHistoricoProducto).HasName("historico_producto_pkey");

            entity.ToTable("historico_producto");

            entity.Property(e => e.IdHistoricoProducto)
                .HasDefaultValueSql("nextval('seq_historico_producto'::regclass)")
                .HasColumnName("id_historico_producto");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Impuesto)
                .HasPrecision(15, 2)
                .HasColumnName("impuesto");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(200)
                .HasColumnName("numero_despacho");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(15, 2)
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(15, 2)
                .HasColumnName("precio_venta");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdImpuesto).HasName("impuesto_pkey");

            entity.ToTable("impuesto");

            entity.Property(e => e.IdImpuesto)
                .HasDefaultValueSql("nextval('seq_impuesto_id'::regclass)")
                .HasColumnName("id_impuesto");
            entity.Property(e => e.CodigoPorcentajeSri)
                .HasColumnType("character varying")
                .HasColumnName("codigo_porcentaje_sri");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasColumnName("codigo_sri");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("porcentaje");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasColumnName("tipo");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("inventario_pkey");

            entity.ToTable("inventario");

            entity.Property(e => e.IdInventario)
                .HasDefaultValueSql("nextval('seq_inventario_id'::regclass)")
                .HasColumnName("id_inventario");
            entity.Property(e => e.Cantidad)
                .HasPrecision(10, 2)
                .HasColumnName("cantidad");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasColumnName("descuento");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(100)
                .HasColumnName("factura_numero");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_movimiento");
            entity.Property(e => e.IdCuentaContable).HasColumnName("id_cuenta_contable");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Iva)
                .HasPrecision(10, 2)
                .HasColumnName("iva");
            entity.Property(e => e.NumeroDespacho)
                .HasMaxLength(50)
                .HasColumnName("numero_despacho");
            entity.Property(e => e.PrecioCalculo)
                .HasPrecision(10, 2)
                .HasColumnName("precio_calculo");
            entity.Property(e => e.PrecioUnitario)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario");
            entity.Property(e => e.PrecioUnitarioFinal)
                .HasPrecision(10, 2)
                .HasColumnName("precio_unitario_final");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.SubTotal)
                .HasPrecision(10, 2)
                .HasColumnName("sub_total");
            entity.Property(e => e.Subtotal15)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal15");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(1)
                .HasColumnName("tipo_movimiento");
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .HasColumnName("total");
            entity.Property(e => e.TransaccionRegistrada)
                .HasDefaultValue(false)
                .HasColumnName("transaccion_registrada");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("inventario_id_producto_fkey");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("sucursal_inventario_fk");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("menu_pkey");

            entity.ToTable("menu");

            entity.Property(e => e.IdMenu)
                .HasDefaultValueSql("nextval('seq_menu'::regclass)")
                .HasColumnName("id_menu");
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
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.MenuId).HasColumnName("menu_id");
            entity.Property(e => e.Url)
                .HasMaxLength(100)
                .HasColumnName("url");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<MenuPerfil>(entity =>
        {
            entity.HasKey(e => e.IdMenuPerfil).HasName("menu_perfil_pkey");

            entity.ToTable("menu_perfil");

            entity.Property(e => e.IdMenuPerfil)
                .HasDefaultValueSql("nextval('seq_menu_perfil'::regclass)")
                .HasColumnName("id_menu_perfil");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdMenu).HasColumnName("id_menu");
            entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdNotaCredito).HasName("nota_credito_pkey");

            entity.ToTable("nota_credito");

            entity.Property(e => e.IdNotaCredito)
                .HasDefaultValueSql("nextval('seq_nota_credito'::regclass)")
                .HasColumnName("id_nota_credito");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasColumnName("clave_acceso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaAutorizacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_autorizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.Motivo)
                .HasMaxLength(200)
                .HasColumnName("motivo");
            entity.Property(e => e.NumeroAutorizacion)
                .HasMaxLength(200)
                .HasColumnName("numero_autorizacion");
            entity.Property(e => e.NumeroNota)
                .HasMaxLength(200)
                .HasColumnName("numero_nota");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Xml)
                .HasMaxLength(1000000)
                .HasColumnName("xml");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.NotaCreditos)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("nota_credito_id_empresa_fkey");
            
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("pago_pkey");

            entity.ToTable("pago");

            entity.Property(e => e.IdPago)
                .HasDefaultValueSql("nextval('seq_pago'::regclass)")
                .HasColumnName("id_pago");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdPaquete).HasName("paquete_pkey");

            entity.ToTable("paquete");

            entity.Property(e => e.IdPaquete)
                .HasDefaultValueSql("nextval('seq_paquete'::regclass)")
                .HasColumnName("id_paquete");
            entity.Property(e => e.CantidadEmisores).HasColumnName("cantidad_emisores");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
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
            entity.HasKey(e => e.IdPaqueteContador).HasName("paquete_contador_pkey");

            entity.ToTable("paquete_contador");

            entity.Property(e => e.IdPaqueteContador)
                .HasDefaultValueSql("nextval('seq_paquete_contador'::regclass)")
                .HasColumnName("id_paquete_contador");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdParametro).HasName("parametro_pkey");

            entity.ToTable("parametro");

            entity.Property(e => e.IdParametro)
                .HasDefaultValueSql("nextval('seq_parametro'::regclass)")
                .HasColumnName("id_parametro");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.NombreParametro)
                .HasMaxLength(100)
                .HasColumnName("nombre_parametro");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Valor)
                .HasMaxLength(100)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Parametros)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("parametro_id_empresa_fkey");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.IdPerfil).HasName("perfil_pkey");

            entity.ToTable("perfil");

            entity.Property(e => e.IdPerfil)
                .HasDefaultValueSql("nextval('seq_perfil'::regclass)")
                .HasColumnName("id_perfil");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("persona_pkey");

            entity.ToTable("persona");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_persona");

            entity.HasIndex(e => e.IdTipoIdentificacion, "fki_tipo_identiificacion_persona_fk");

            entity.Property(e => e.IdPersona)
                .HasDefaultValueSql("nextval('seq_persona'::regclass)")
                .HasColumnName("id_persona");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdTipoIdentificacion).HasColumnName("id_tipo_identificacion");
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

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_persona");

            entity.HasOne(d => d.IdTipoIdentificacionNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdTipoIdentificacion)
                .HasConstraintName("tipo_identiificacion_persona_fk");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("producto_pkey");

            entity.ToTable("producto");

            entity.HasIndex(e => e.IdEmpresa, "fki_fk_producto_empresa");

            entity.HasIndex(e => e.IdImpuesto, "fki_i");

            entity.HasIndex(e => e.Codigo, "producto_codigo_key").IsUnique();

            entity.Property(e => e.IdProducto)
                .HasDefaultValueSql("nextval('seq_producto_id'::regclass)")
                .HasColumnName("id_producto");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Descuento)
                .HasPrecision(10, 2)
                .HasColumnName("descuento%");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
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
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(10, 2)
                .HasColumnName("precio_venta");
            entity.Property(e => e.Stock)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("stock");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.Utilidad).HasColumnName("utilidad%");

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
            entity.HasKey(e => e.IdProductoProveedor).HasName("producto_proveedor_pkey");

            entity.ToTable("producto_proveedor");

            entity.Property(e => e.IdProductoProveedor)
                .HasDefaultValueSql("nextval('seq_producto_proveedor_id'::regclass)")
                .HasColumnName("id_producto_proveedor");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.PrecioCompra)
                .HasPrecision(10, 2)
                .HasColumnName("precio_compra");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("producto_proveedor_id_producto_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_proveedor");

            entity.Property(e => e.IdProveedor)
                .HasDefaultValueSql("nextval('seq_proveedor'::regclass)")
                .HasColumnName("id_proveedor");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(15)
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.RetencionIva)
                .HasPrecision(15, 2)
                .HasColumnName("retencion_iva");
            entity.Property(e => e.RetencionPorcentaje)
                .HasPrecision(15, 2)
                .HasColumnName("retencion_porcentaje");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Proveedors)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("empresa_proveedor");
        });

        modelBuilder.Entity<Retencion>(entity =>
        {
            entity.HasKey(e => e.IdRetencion).HasName("retencion_pkey");

            entity.ToTable("retencion");

            entity.Property(e => e.IdRetencion)
                .HasDefaultValueSql("nextval('seq_retencion'::regclass)")
                .HasColumnName("id_retencion");
            entity.Property(e => e.BaseImponible)
                .HasPrecision(15, 2)
                .HasColumnName("base_imponible");
            entity.Property(e => e.ClaveAcceso)
                .HasMaxLength(200)
                .HasColumnName("clave_acceso");
            entity.Property(e => e.ComprobanteRetencion)
                .HasMaxLength(50)
                .HasColumnName("comprobante_retencion");
            entity.Property(e => e.EjercicioFiscal)
                .HasMaxLength(50)
                .HasColumnName("ejercicio_fiscal");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaAutorizacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_autorizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
            entity.Property(e => e.Impuesto)
                .HasMaxLength(50)
                .HasColumnName("impuesto");
            entity.Property(e => e.NumeroAutorizacion)
                .HasMaxLength(200)
                .HasColumnName("numero_autorizacion");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .HasColumnName("numero_factura");
            entity.Property(e => e.PorcentajeRetencion)
                .HasPrecision(15, 2)
                .HasColumnName("porcentaje_retencion");
            entity.Property(e => e.TipoContribuyente)
                .HasMaxLength(100)
                .HasColumnName("tipo_contribuyente");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
            entity.Property(e => e.ValorRetenido)
                .HasPrecision(15, 2)
                .HasColumnName("valor_retenido");
            entity.Property(e => e.Xml)
                .HasMaxLength(1000000)
                .HasColumnName("xml");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Retencions)
                .HasForeignKey(d => d.IdEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("retencion_id_empresa_fkey");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("sucursal_pkey");

            entity.ToTable("sucursal");

            entity.Property(e => e.IdSucursal)
                .HasDefaultValueSql("nextval('seq_sucursal'::regclass)")
                .HasColumnName("id_sucursal");
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .HasColumnName("clave");
            entity.Property(e => e.DireccionSucursal)
                .HasMaxLength(100)
                .HasColumnName("direccion_sucursal");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmisor).HasColumnName("id_emisor");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.NombreSucursal)
                .HasMaxLength(100)
                .HasColumnName("nombre_sucursal");
            entity.Property(e => e.PuntoEmision)
                .HasMaxLength(50)
                .HasColumnName("punto_emision");
            entity.Property(e => e.Secuencial)
                .HasMaxLength(20)
                .HasColumnName("secuencial");
            entity.Property(e => e.Telefono)
                .HasMaxLength(13)
                .HasColumnName("telefono");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .HasColumnName("usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdSucursalFactura).HasName("sucursal_factura_pkey");

            entity.ToTable("sucursal_factura");

            entity.Property(e => e.IdSucursalFactura)
                .HasDefaultValueSql("nextval('seq_sucursal_factura'::regclass)")
                .HasColumnName("id_sucursal_factura");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdFactura).HasColumnName("id_factura");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdSucursalInventario).HasName("sucursal_inventario_pkey");

            entity.ToTable("sucursal_inventario");

            entity.Property(e => e.IdSucursalInventario)
                .HasDefaultValueSql("nextval('seq_sucursal_inventario'::regclass)")
                .HasColumnName("id_sucursal_inventario");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdInventario).HasColumnName("id_inventario");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdTipoIdemtificacion).HasName("tipo_identificacion_pkey");

            entity.ToTable("tipo_identificacion");

            entity.Property(e => e.IdTipoIdemtificacion)
                .HasDefaultValueSql("nextval('seq_tipo_identificacion'::regclass)")
                .HasColumnName("id_tipo_idemtificacion");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasColumnName("codigo_sri");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<TipoPago>(entity =>
        {
            entity.HasKey(e => e.IdTipoPago).HasName("tipo_pago_pkey");

            entity.ToTable("tipo_pago");

            entity.Property(e => e.IdTipoPago).HasColumnName("id_tipo_pago");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(10)
                .HasColumnName("codigo_sri");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoTransaccion>(entity =>
        {
            entity.HasKey(e => e.IdTipoTransaccion).HasName("tipo_transaccion_pkey");

            entity.ToTable("tipo_transaccion");

            entity.Property(e => e.IdTipoTransaccion).HasColumnName("id_tipo_transaccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Tipocuentum>(entity =>
        {
            entity.HasKey(e => e.IdTipoCuenta).HasName("tipocuenta_pkey");

            entity.ToTable("tipocuenta");

            entity.Property(e => e.IdTipoCuenta).HasColumnName("id_tipo_cuenta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("transaccion_pkey");

            entity.ToTable("transaccion");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_transaccion");

            entity.Property(e => e.IdTransaccion)
                .HasDefaultValueSql("nextval('seq_transaccion'::regclass)")
                .HasColumnName("id_transaccion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdInventario).HasColumnName("id_inventario");
            entity.Property(e => e.IdTipoTransaccion).HasColumnName("id_tipo_transaccion");
            entity.Property(e => e.Monto)
                .HasPrecision(15, 2)
                .HasColumnName("monto");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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

        modelBuilder.Entity<UnidadMedidum>(entity =>
        {
            entity.HasKey(e => e.IdUnidadMedida).HasName("unidad_medida_pkey");

            entity.ToTable("unidad_medida");

            entity.Property(e => e.IdUnidadMedida)
                .HasDefaultValueSql("nextval('seq_unidad_medida_id'::regclass)")
                .HasColumnName("id_unidad_medida");
            entity.Property(e => e.Abreviatura)
                .HasMaxLength(10)
                .HasColumnName("abreviatura");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.IdEmpresa, "fki_empresa_usuario");

            entity.Property(e => e.IdUsuario)
                .HasDefaultValueSql("nextval('seq_usuario'::regclass)")
                .HasColumnName("id_usuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(100)
                .HasColumnName("clave");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdUsuarioPerfil).HasName("usuario_perfil_pkey");

            entity.ToTable("usuario_perfil");

            entity.Property(e => e.IdUsuarioPerfil)
                .HasDefaultValueSql("nextval('seq_usuario_perfil'::regclass)")
                .HasColumnName("id_usuario_perfil");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPerfil).HasColumnName("id_perfil");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdUsuarioSucursal).HasName("usuario_sucursal_pkey");

            entity.ToTable("usuario_sucursal");

            entity.Property(e => e.IdUsuarioSucursal)
                .HasDefaultValueSql("nextval('seq_usuario_sucursal'::regclass)")
                .HasColumnName("id_usuario_sucursal");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

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
            entity.HasKey(e => e.IdVentaPaquete).HasName("venta_paquete_pkey");

            entity.ToTable("venta_paquete");

            entity.Property(e => e.IdVentaPaquete)
                .HasDefaultValueSql("nextval('seq_venta_paquete'::regclass)")
                .HasColumnName("id_venta_paquete");
            entity.Property(e => e.EstadoBoolean)
                .HasDefaultValue(true)
                .HasColumnName("estado_boolean");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modificacion");
            entity.Property(e => e.IdPaquete).HasColumnName("id_paquete");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UsuarioCreacion).HasColumnName("usuario_creacion");
            entity.Property(e => e.UsuarioModificacion).HasColumnName("usuario_modificacion");

            entity.HasOne(d => d.IdPaqueteNavigation).WithMany(p => p.VentaPaquetes)
                .HasForeignKey(d => d.IdPaquete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venta_paquete_id_paquete_fkey");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.VentaPaquetes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venta_paquete_id_usuario_fkey");
        });
        modelBuilder.HasSequence("seq_anticipo");
        modelBuilder.HasSequence("seq_anticipo_cuenta");
        modelBuilder.HasSequence("seq_categoria_producto_id");
        modelBuilder.HasSequence("seq_cliente");
        modelBuilder.HasSequence("seq_comision");
        modelBuilder.HasSequence("seq_comision_contador");
        modelBuilder.HasSequence("seq_cuenta");
        modelBuilder.HasSequence("seq_cuenta_cobrar");
        modelBuilder.HasSequence("seq_despacho");
        modelBuilder.HasSequence("seq_detalle_despacho");
        modelBuilder.HasSequence("seq_detalle_factura");
        modelBuilder.HasSequence("seq_emisor");
        modelBuilder.HasSequence("seq_empresa");
        modelBuilder.HasSequence("seq_factura");
        modelBuilder.HasSequence("seq_historico_producto");
        modelBuilder.HasSequence("seq_impuesto_id");
        modelBuilder.HasSequence("seq_inventario_id");
        modelBuilder.HasSequence("seq_menu");
        modelBuilder.HasSequence("seq_menu_perfil");
        modelBuilder.HasSequence("seq_nota_credito");
        modelBuilder.HasSequence("seq_pago");
        modelBuilder.HasSequence("seq_paquete");
        modelBuilder.HasSequence("seq_paquete_contador");
        modelBuilder.HasSequence("seq_parametro");
        modelBuilder.HasSequence("seq_perfil");
        modelBuilder.HasSequence("seq_persona");
        modelBuilder.HasSequence("seq_producto_id");
        modelBuilder.HasSequence("seq_producto_proveedor_id");
        modelBuilder.HasSequence("seq_proveedor");
        modelBuilder.HasSequence("seq_retencion");
        modelBuilder.HasSequence("seq_sucursal");
        modelBuilder.HasSequence("seq_sucursal_factura");
        modelBuilder.HasSequence("seq_sucursal_inventario");
        modelBuilder.HasSequence("seq_tipo_cuenta");
        modelBuilder.HasSequence("seq_tipo_identificacion");
        modelBuilder.HasSequence("seq_tipo_pago");
        modelBuilder.HasSequence("seq_tipo_transaccion");
        modelBuilder.HasSequence("seq_transaccion");
        modelBuilder.HasSequence("seq_unidad_medida_id");
        modelBuilder.HasSequence("seq_usuario");
        modelBuilder.HasSequence("seq_usuario_perfil");
        modelBuilder.HasSequence("seq_usuario_sucursal");
        modelBuilder.HasSequence("seq_venta_paquete");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
