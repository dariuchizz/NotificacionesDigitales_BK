using Common.IRepositories;
using System.Threading.Tasks;

namespace Common.IServices
{
    public interface IUnitOfWorkNotificacion
    {
        Task<int> SaveChangeAsync();

        ISuministroRepository CuentaRepository();

        ITipoComunicacionRepository TipoComunicacionRepository();

        IEventoEmailRepository EventoEmailRepository();

        IEventoSMSRepository EventoSMSRepository();

        IStoreRepository ViewRepository();

        IComunicacionRepository ComunicacionRepository();

        IProcesoEventoRepository ProcesoEventoRepository();

        IFacturaRepository FacturaRepository();

        IAvisoDeudaRepository AvisoDeudaRepository();

        ICambioContactoRepository CambioContactoRepository();

        IEmailRepository EmailRepository();

        ICelularRepository CelularRepository();

        ICobrabilidadRepository CobrabilidadRepository();

        ICanalRepository CanalRepository();

        IListaGrisRepository ListaGrisRepository();

        IMotivoBajaListaGrisRepository MotivoBajaListaGrisRepository();

        IEventosAccionesRepository EventosAccionesRepository();

        IEventosResultantesEmailRepository EventosResultantesEmailRepository();

        ISuministroRepository SuministroRepository();

        IEventosComunicacionesRepository EventosComunicacionesRepository();

        ICampaniaRepository CampaniaRepository();

        IMotivoBajaRepository MotivoBajaRepository();

        IEstadoCampaniasRepository EstadoCampaniasRepository();

        IEstadosSuministroRepository EstadosSuministroRepository();

        ICategoriasSuministroRepository CategoriasSuministroRepository();

        IParametrosCampaniasRepository ParametrosCampaniasRepository();

        ICsvCampaniaRepository CsvCampaniaRepository();

        IStoreRepository StoreRepository();

        IClienteGrupoRepository ClienteCategoriasRepository();

        ICampaniasHtmlVariableRepository CampaniasHtmlVariableRepository();

        ITipoCampaniaRepository TipoCampaniaRepository();

        IEnvioSMSGenericoRepository EnvioSMSGenericoRepository();

        ISistemaRepository SistemaRepository();

        IProcesoNegocioRepository ProcesoNegocioRepository();

        IClaseCampaniaRepository ClaseCampaniaRepository();

        INotificacionHuemulRepository NotificacionHuemulRepository();

        void Dispose();
        
        void Connect();
    }
}