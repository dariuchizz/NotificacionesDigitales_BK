using Common.IRepositories;
using Common.IServices;
using Common.Model.NotificacionesDigitales;
using Common.Repositories;
using System.Threading.Tasks;

namespace Common.Services
{
    public class UnitOfWorkNotificacion : IUnitOfWorkNotificacion
    {
        private readonly INotificacionesDigitalesDbContext _context;

        public UnitOfWorkNotificacion(INotificacionesDigitalesDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangeAsync()
        {
            var response = await _context.SaveChangesAsync();
            return response;
        }

        public ISuministroRepository CuentaRepository()
        {
            var repo = new SuministroRepository(_context);
            return repo;
        }

        public ITipoComunicacionRepository TipoComunicacionRepository()
        {
            var repo = new TipoComunicacionRepository(_context);
            return repo;
        }

        public IEventoEmailRepository EventoEmailRepository()
        {
            var repo = new EventoEmailRepository(_context);
            return repo;
        }

        public IEventoSMSRepository EventoSMSRepository()
        {
            var repo = new EventoSMSRepository(_context);
            return repo;
        }

        public IStoreRepository ViewRepository()
        {
            var repo = new StoreRepository(_context);
            return repo;
        }

        public IComunicacionRepository ComunicacionRepository()
        {
            var repo = new ComunicacionRepository(_context);
            return repo;
        }

        public IProcesoEventoRepository ProcesoEventoRepository()
        {
            var repo = new ProcesoEventoRepository(_context);
            return repo;
        }

        public ICambioContactoRepository CambioContactoRepository()
        {
            var repo = new CambioContactoRepository(_context);
            return repo;
        }

        public IFacturaRepository FacturaRepository()
        {
            var repo = new FacturaRepository(_context);
            return repo;
        }

        public IAvisoDeudaRepository AvisoDeudaRepository()
        {
            var repo = new AvisoDeudaRepository(_context);
            return repo;
        }

        public IEmailRepository EmailRepository()
        {
            var repo = new EmailRepository(_context);
            return repo;
        }

        public ICelularRepository CelularRepository()
        {
            var repo = new CelularRepository(_context);
            return repo;
        }

        public ICobrabilidadRepository CobrabilidadRepository()
        {
            var repo = new CobrabilidadRepository(_context);
            return repo;
        }

        public ICanalRepository CanalRepository()
        {
            var repo = new CanalRepository(_context);
            return repo;
        }

        public IListaGrisRepository ListaGrisRepository()
        {
            var repo = new ListaGrisRepository(_context);
            return repo;
        }

        public IMotivoBajaListaGrisRepository MotivoBajaListaGrisRepository()
        {
            var repo = new MotivoBajaListaGrisRepository(_context);
            return repo;
        }

        public IEventosAccionesRepository EventosAccionesRepository()
        {
            var repo = new EventosAccionesRepository(_context);
            return repo;
        }

        public IEventosResultantesEmailRepository EventosResultantesEmailRepository()
        {
            var repo = new EventosResultantesEmailsRepository(_context);
            return repo;
        }

        public ISuministroRepository SuministroRepository()
        {
            var repo = new SuministroRepository(_context);
            return repo;
        }

        public IEventosComunicacionesRepository EventosComunicacionesRepository()
        {
            var repo = new EventosComunicacionesRepository(_context);
            return repo;
        }

        public ICampaniaRepository CampaniaRepository()
        {
            var repo = new CampaniaRepository(_context);
            return repo;
        }

        public IMotivoBajaRepository MotivoBajaRepository()
        {
            var repo = new MotivoBajaRepository(_context);
            return repo;
        }

        public IEstadoCampaniasRepository EstadoCampaniasRepository()
        {
            var repo = new EstadoCampaniasRepository(_context);
            return repo;
        }

        public IEstadosSuministroRepository EstadosSuministroRepository()
        {
            var repo = new EstadosSuministroRepository(_context);
            return repo;
        }

        public ICategoriasSuministroRepository CategoriasSuministroRepository()
        {
            var repo = new CategoriasSuministroRepository(_context);
            return repo;
        }

        public IParametrosCampaniasRepository ParametrosCampaniasRepository()
        {
            var repo = new ParametrosCampaniaRepository(_context);
            return repo;
        }

        public ICsvCampaniaRepository CsvCampaniaRepository()
        {
            var repo = new CsvCampaniaRepository(_context);
            return repo;
        }

        public IStoreRepository StoreRepository()
        {
            var repo = new StoreRepository(_context);
            return repo;
        }

        public IClienteGrupoRepository ClienteCategoriasRepository()
        {
            var repo = new ClienteGrupoRepository(_context);
            return repo;
        }
        public void Connect()
        {
            _context.Connection().Open();
        }

        public ICampaniasHtmlVariableRepository CampaniasHtmlVariableRepository()
        {
            var repo = new CampaniasHtmlVariableRepository(_context);
            return repo;
        }

        public ITipoCampaniaRepository TipoCampaniaRepository()
        {
            var repo = new TipoCampaniaRepository(_context);
            return repo;
        }

        public IEnvioSMSGenericoRepository EnvioSMSGenericoRepository()
        {
            var repo = new EnvioSMSGenericoRepository(_context);
            return repo;
        }

        public ISistemaRepository SistemaRepository()
        {
            var repo = new SistemaRepository(_context);
            return repo;
        }

        public IProcesoNegocioRepository ProcesoNegocioRepository()
        {
            var repo = new ProcesoNegocioRepository(_context);
            return repo;
        }

        public IClaseCampaniaRepository ClaseCampaniaRepository()
        {
            var repo = new ClaseCampaniaRepository(_context);
            return repo;
        }

        public INotificacionHuemulRepository NotificacionHuemulRepository()
        {
            var repo = new NotificacionHuemulRepository(_context);
            return repo;
        }

        public void Dispose()
        {
            _context.Connection().Dispose();
        }

        
    }
}
