using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace Common.Model.NotificacionesDigitales
{

    public class NotificacionesDigitalesDbContext : DbContext, INotificacionesDigitalesDbContext
    {
        public NotificacionesDigitalesDbContext(DbContextOptions<NotificacionesDigitalesDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<AvisosDeuda> AvisosDeuda { get; set; }
        
        public DbSet<AvisosDeudasDetalle> AvisosDeudasDetalle { get; set; }

        public DbSet<CambioContacto> CambiosContactos { get; set; }

        public DbSet<Canal> Canal { get; set; }

        public DbSet<Celular> Celular { get; set; }

        public DbSet<Comunicacion> Comunicacion { get; set; }

        public DbSet<ConfiguracionEmail> ConfiguracionEmail { get; set; }

        public DbSet<Email> Email { get; set; }

        public DbSet<Envio> Envios { get; set; }

        public DbSet<EventoEmail> EventoEmail { get; set; }

        public DbSet<EventoSMS> EventoSMS { get; set; }

        public DbSet<Factura> Factura { get; set; }

        public DbSet<ListaGris> ListaGris { get; set; }

        public DbSet<MotivoBaja> MotivoBaja { get; set; }
        
        public DbSet<MotivoBajaListaGris> MotivoBajaListaGris { get; set; }

        public DbSet<ProcesoEvento> ProcesoEvento { get; set; }

        public DbSet<ReenvioEmailFallado> ReenvioEmailFallado { get; set; }

        public DbSet<RptNotificaciones> RptNotificaciones { get; set; }

        public DbSet<RptNotificacionesDetalle> RptNotificacionesDetalles { get; set; }

        public DbSet<Segmento> Segmento { get; set; }

        public DbSet<SegmentoEmail> SegmentoEmail { get; set; }

        public DbSet<Suministro> Suministro { get; set; }

        public DbSet<TipoComunicacion> TipoComunicacion { get; set; }

        public DbSet<EventosResultantesEmail> EventosResultantesEmail { get; set; }

        public DbSet<EventosResultantes> EventosResultantes { get; set; }

        public DbSet<EventosComunicaciones> EventosComunicaciones { get; set; }
        
        public DbSet<Campania> Campania { get; set; }
        
        public DbSet<EstadoCampania> EstadoCampania { get; set; }
        
        public DbSet<ParametrosCampania> ParametrosCampania { get; set; }
        
        public DbSet<TipoCampania> TipoCampania { get; set; }
        
        public DbSet<CsvCampania> CsvCampania { get; set; }
        
        public DbSet<TipoEnvio> TipoEnvio { get; set; }

        public DbSet<EstadosSuministro> EstadosSuministro { get; set; }

        public DbSet<CategoriasSuministro> CategoriasSuministro { get; set; }

        public DbSet<ClienteGrupoCategoria> ClienteGrupoCategorias { get; set; }
        
        public DbSet<GrupoCategoria> GrupoCategorias { get; set; }
        
        public DbSet<CampaniasHtmlVariable> CampaniasHtmlVariables { get; set; }

        public DbSet<Sistema> Sistemas { get; set; }

        public DbSet<EnvioSmsGenerico> EnvioSmsGenericos { get; set; }

        public DbSet<ProcesoNegocio> ProcesosNegocios { get; set; }

        public DbSet<v_ObtenerSuministroCampanias> v_ObtenerSuministroCampanias { get; set; }

        public DbSet<v_ObtenerSuministroProcesosNegocios> v_ObtenerSuministroProcesosNegocios { get; set; }

        public DbSet<NotificacionHuemul> NotificacionHuemul { get; set; }

        public DbSet<ClaseCampania> ClaseCampania { get; set; }

        public IDbConnection Connection()
        {
            return this.Database.GetDbConnection();
        }

        public DatabaseFacade GetDatabase()
        {
            return this.Database;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.CreateKeys(modelBuilder);
            this.CreateRelationships(modelBuilder);
        }
        
        private void CreateKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvisosDeuda>().HasKey(k => k.IdAvisoDeuda);
            modelBuilder.Entity<AvisosDeudasDetalle>().HasKey(k => new { k.IdAvisoDeuda, k.IdFactura });
            modelBuilder.Entity<CambioContacto>().HasKey(k => k.IdCambioContacto);
            modelBuilder.Entity<Canal>().HasKey(k => k.IdCanal);
            modelBuilder.Entity<ClaseCampania>().HasKey(k => k.IdClaseCampania);
            modelBuilder.Entity<Celular>().HasKey(k => k.IdCelular);
            modelBuilder.Entity<Comunicacion>().HasKey(k => k.IdComunicacion);
            modelBuilder.Entity<ConfiguracionEmail>().HasKey(k => k.IdConfiguracionEmail);
            modelBuilder.Entity<Email>().HasKey(k => k.IdEmail);
            modelBuilder.Entity<Envio>().HasKey(k => k.IdEnvio);
            modelBuilder.Entity<EventoEmail>().HasKey(k => k.IdEventoEmail);
            modelBuilder.Entity<EventoSMS>().HasKey(k => k.IdEventoSMS);
            modelBuilder.Entity<Factura>().HasKey(k => k.IdFactura);
            modelBuilder.Entity<ListaGris>().HasKey(k => k.IdListaGris);
            modelBuilder.Entity<MotivoBajaListaGris>().HasKey(k => k.IdMotivoBajaListaGris);
            modelBuilder.Entity<MotivoBaja>().HasKey(k => k.IdMotivoBaja);
            modelBuilder.Entity<ProcesoEvento>().HasKey(k => k.IdProcesoEvento);
            modelBuilder.Entity<ReenvioEmailFallado>().HasKey(k => k.IdReenvioEmailFallado);
            modelBuilder.Entity<RptNotificaciones>()
                .HasKey(k => new { k.IdTipoComunicacion, k.IdEnvio, k.FechaEnvio, k.FechaVencimiento });
            modelBuilder.Entity<RptNotificacionesDetalle>()
                .HasKey(k => new { k.IdTipoComunicacion, k.IdEnvio, k.CantidadDias, k.FechaEnvio, k.FechaVencimiento });
            modelBuilder.Entity<Segmento>().HasKey(k => k.IdSegmento);
            modelBuilder.Entity<SegmentoEmail>().HasKey(k => new { k.IdSegmento, k.IdEmail });
            modelBuilder.Entity<Suministro>().HasKey(k => k.IdSuministro);
            modelBuilder.Entity<EventosResultantesEmail>().HasKey(k => k.IdEventoResultanteEmail);
            modelBuilder.Entity<EventosResultantes>().HasKey(k => k.IdEventoResultante);
            modelBuilder.Entity<EventosComunicaciones>().HasKey(k => k.IdEventoComunicacion);
            modelBuilder.Entity<Campania>().HasKey(k => k.IdCampania);
            modelBuilder.Entity<ParametrosCampania>().HasKey(k => k.IdParametro);
            modelBuilder.Entity<EstadoCampania>().HasKey(k => k.IdEstadoCampania);
            modelBuilder.Entity<TipoCampania>().HasKey(k => k.IdTipoCampania);
            modelBuilder.Entity<CsvCampania>().HasKey(k => k.IdCsvCampania);
            modelBuilder.Entity<TipoComunicacion>().HasKey(k => k.IdTipoComunicacion);
            modelBuilder.Entity<Envio>().HasKey(k => k.IdEnvio);
            modelBuilder.Entity<EstadosSuministro>().HasKey(k => k.IdEstadoSuministro);
            modelBuilder.Entity<CategoriasSuministro>().HasKey(k => k.IdCategoriaSuministro);
            modelBuilder.Entity<ClienteGrupoCategoria>().HasKey(k => k.IdClienteGrupo);
            modelBuilder.Entity<GrupoCategoria>().HasKey(k => new {k.IdGrupoCategoria, k.Categoria});
            modelBuilder.Entity<CampaniasHtmlVariable>().HasKey(k => k.IdCampaniaHtmlVariable);
            modelBuilder.Entity<Sistema>().HasKey(k => k.IdSistema);
            modelBuilder.Entity<EnvioSmsGenerico>().HasKey(k => k.IdEnvioSMSGenerico);
            modelBuilder.Entity<ProcesoNegocio>().HasKey(k => k.IdProcesoNegocio);
            modelBuilder.Entity<NotificacionHuemul>().HasKey(k => k.IdNotificacionHuemul);
            modelBuilder.Entity<v_ObtenerSuministroCampanias>().HasNoKey();
            modelBuilder.Entity<v_ObtenerSuministroProcesosNegocios>().HasNoKey();
        }

        private void CreateRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Canal>()
                .HasMany(c => c.Comunicaciones)
                .WithOne(c => c.Canale)
                .HasForeignKey(k => k.IdCanal);
            modelBuilder.Entity<Suministro>()
                .HasMany(m => m.Emails)
                .WithOne(o => o.Suministro)
                .HasForeignKey(f => f.IdSuministro);
            modelBuilder.Entity<Suministro>()
                .HasMany(m => m.Facturas)
                .WithOne(o => o.Suministro)
                .HasForeignKey(f => f.IdSuministro);
            modelBuilder.Entity<Email>()
                .HasMany(s => s.SegmentosEmails)
                .WithOne(e => e.Email)
                .HasForeignKey(k => k.IdEmail);
            modelBuilder.Entity<MotivoBaja>()
                .HasMany(e => e.Emails)
                .WithOne(m => m.MotivosBaja)
                .HasForeignKey(k => k.IdMotivoBaja);
            modelBuilder.Entity<Segmento>().HasMany(s => s.SegmentosEmails)
                .WithOne(s => s.Segmento)
                .HasForeignKey(k => k.IdSegmento);
            modelBuilder.Entity<AvisosDeuda>()
                .HasMany(m => m.AvisosDeudasDetalles)
                .WithOne(o => o.AvisosDeuda)
                .HasForeignKey(k => k.IdAvisoDeuda);
            modelBuilder.Entity<Factura>()
                .HasMany(m => m.AvisosDeudasDetalles)
                .WithOne(o => o.Factura)
                .HasForeignKey(k => k.IdFactura);
            modelBuilder.Entity<RptNotificaciones>()
                .HasMany(m => m.RptNotificacionesDetalles)
                .WithOne(r => r.RptNotificaciones)
                .HasForeignKey(k => new { k.IdTipoComunicacion, k.IdEnvio, k.FechaEnvio, k.FechaVencimiento });

            modelBuilder.Entity<TipoComunicacion>()
                .HasMany(m => m.Envios)
                .WithOne(o => o.TipoComunicacion)
                .HasForeignKey(k => k.IdRelacionado);

            modelBuilder.Entity<Envio>()
                .HasOne(o => o.Campania)
                .WithOne(o => o.Envio)
                .HasForeignKey<Envio>(k => k.IdRelacionado);

            modelBuilder.Entity<Comunicacion>()
                .HasMany(m => m.EventosComunicaciones)
                .WithOne(w => w.Comunicacion)
                .HasForeignKey(k => k.IdComunicacion);
            modelBuilder.Entity<EventosResultantes>()
                .HasMany(m => m.EventosComunicaciones)
                .WithOne(w => w.EventosResultante)
                .HasForeignKey(k => k.IdEventoResultante);
            modelBuilder.Entity<Campania>()
                .HasMany(m => m.ParametrosCampanias)
                .WithOne(o => o.Campania)
                .HasForeignKey(k => k.IdCampania);
            modelBuilder.Entity<Campania>()
                .HasMany(m => m.CsvCampanias)
                .WithOne(o => o.Campania)
                .HasForeignKey(k => k.IdCampania);
            modelBuilder.Entity<TipoCampania>()
                .HasMany(m => m.Campanias)
                .WithOne(o => o.TipoCampania)
                .HasForeignKey(k => k.IdTipoCampania);
            modelBuilder.Entity<EstadoCampania>()
                .HasMany(m => m.Campanias)
                .WithOne(o => o.EstadoCampania)
                .HasForeignKey(k => k.IdEstadoCampania);
            modelBuilder.Entity<ClaseCampania>()
                .HasMany(m => m.Campanias)
                .WithOne(o => o.ClaseCampania)
                .HasForeignKey(k => k.IdClaseCampania);
            modelBuilder.Entity<TipoEnvio>()
                .HasMany(m => m.Envios)
                .WithOne(o => o.TipoEnvio)
                .HasForeignKey(k => k.IdTipoEnvio);

            modelBuilder.Entity<CsvCampania>()
                .HasOne(o => o.Canal)
                .WithMany(m => m.CsvCampanias)
                .HasForeignKey(k => k.IdCanal);

            modelBuilder.Entity<Comunicacion>()
                .HasMany(m => m.ListasGrises)
                .WithOne(o => o.Comunicacion)
                .HasForeignKey(k => k.IdComunicacion);
            modelBuilder.Entity<ClienteGrupoCategoria>()
                .HasMany(m => m.GrupoCategorias)
                .WithOne(o => o.ClienteGrupoCategoria)
                .HasForeignKey(k => k.IdGrupoCategoria);
            modelBuilder.Entity<Campania>()
                .HasMany(m => m.CampaniasHtmlVariables)
                .WithOne(o => o.Campania)
                .HasForeignKey(k => k.IdCampania);
        }
    }
}