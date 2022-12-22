using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Common.Model.NotificacionesDigitales
{
    public interface INotificacionesDigitalesDbContext
    {
        DbSet<AvisosDeuda> AvisosDeuda { get; set; }

        DbSet<AvisosDeudasDetalle> AvisosDeudasDetalle { get; set; }

        DbSet<CambioContacto> CambiosContactos { get; set; }

        DbSet<Canal> Canal { get; set; }        

        DbSet<Celular> Celular { get; set; }

        DbSet<Comunicacion> Comunicacion { get; set; }

        DbSet<Email> Email { get; set; }
        
        DbSet<Envio> Envios { get; set; }

        DbSet<EventoEmail> EventoEmail { get; set; }

        DbSet<EventoSMS> EventoSMS { get; set; }

        DbSet<Factura> Factura { get; set; }

        DbSet<ListaGris> ListaGris { get; set; }

        DbSet<MotivoBaja> MotivoBaja { get; set; }

        DbSet<MotivoBajaListaGris> MotivoBajaListaGris { get; set; }

        DbSet<ProcesoEvento> ProcesoEvento { get; set; }

        DbSet<RptNotificaciones> RptNotificaciones { get; set; }

        DbSet<RptNotificacionesDetalle> RptNotificacionesDetalles { get; set; }

        DbSet<Segmento> Segmento { get; set; }

        DbSet<SegmentoEmail> SegmentoEmail { get; set; }

        DbSet<Suministro> Suministro { get; set; }

        DbSet<TipoComunicacion> TipoComunicacion { get; set; }
        
        DbSet<ConfiguracionEmail> ConfiguracionEmail { get; set; }

        DbSet<EventosResultantesEmail> EventosResultantesEmail { get; set; }

        DbSet<EventosResultantes> EventosResultantes { get; set; }

        DbSet<EventosComunicaciones> EventosComunicaciones { get; set; }
        
        DbSet<TipoEnvio> TipoEnvio { get; set; }
        
        DbSet<Campania> Campania { get; set; }

        DbSet<ClaseCampania> ClaseCampania { get; set; }

        DbSet<EstadoCampania> EstadoCampania { get; set; }
        
        DbSet<ParametrosCampania> ParametrosCampania { get; set; }
        
        DbSet<TipoCampania> TipoCampania { get; set; }
        
        DbSet<CsvCampania> CsvCampania { get; set; }
        
        DbSet<EstadosSuministro> EstadosSuministro { get; set; }
        
        DbSet<CategoriasSuministro> CategoriasSuministro { get; set; }
        
        DbSet<ClienteGrupoCategoria> ClienteGrupoCategorias { get; set; }
        
        DbSet<GrupoCategoria> GrupoCategorias { get; set; }
        
        DbSet<CampaniasHtmlVariable> CampaniasHtmlVariables { get; set; }

        DbSet<Sistema> Sistemas { get; set; }

        DbSet<EnvioSmsGenerico> EnvioSmsGenericos { get; set; }

        DbSet<ProcesoNegocio> ProcesosNegocios { get; set; }

        DbSet<v_ObtenerSuministroCampanias> v_ObtenerSuministroCampanias { get; set; }

        DbSet<v_ObtenerSuministroProcesosNegocios> v_ObtenerSuministroProcesosNegocios { get; set; }

        DbSet<NotificacionHuemul> NotificacionHuemul { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        
        DatabaseFacade GetDatabase();
        
        DbSet<T> Set<T>() where T : class;
        
        EntityEntry<T> Entry<T>(T entity) where T : class;
        
        IDbConnection Connection();
    }
}