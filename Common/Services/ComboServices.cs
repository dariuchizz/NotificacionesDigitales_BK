using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.Services
{
    public class ComboServices : ICombosServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWorkNotificacion;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkDirectory _unitOfWorkDirectory;

        public ComboServices(IUnitOfWorkNotificacion unitOfWorkNotificacion, IMapper mapper, IUnitOfWorkDirectory unitOfWorkDirectory)
        {
            _unitOfWorkNotificacion = unitOfWorkNotificacion;
            _mapper = mapper;
            _unitOfWorkDirectory = unitOfWorkDirectory;
        }
        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboEventosResultantesEmailsAsync()
        {
            var entity = await _unitOfWorkNotificacion.EventosResultantesEmailRepository().GetAllAsync();
            var mapper = entity.Select(s => _mapper.Map<ComboLongDto>(s));
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }
        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboCanalesAsync()
        {
            var entity = await _unitOfWorkNotificacion.CanalRepository().GetAllAsync();
            var mapper = entity.OrderBy(o => o.Descripcion).Select(s => _mapper.Map<ComboLongDto>(s));
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }
        public async Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboEstadosCampaniaAsync()
        {
            var entity = await _unitOfWorkNotificacion.EstadoCampaniasRepository().GetAllAsync();
            var mapper = entity.OrderBy(o => o.Descripcion).Select(s => _mapper.Map<ComboLongDto>(s));
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboEstadosSuministroAsync()
        {
            var entity = await _unitOfWorkNotificacion.EstadosSuministroRepository().GetAllAsync();
            var mapper = entity.OrderBy(o => o.Codigo).Select(s => _mapper.Map<ComboStringDto>(s)).OrderBy(o => o.Id).AsEnumerable();
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboCategoriasSuministroAsync()
        {
            var entity = await _unitOfWorkNotificacion.CategoriasSuministroRepository().GetAllAsync();
            var mapper = entity.OrderBy(o => o.Codigo).Select(s => _mapper.Map<ComboStringDto>(s)).OrderBy(o => o.Id).AsEnumerable();
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }
        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboLocalidadesByBusinessUnitAsync(List<string> businessUnits)
        {
            var entity = await _unitOfWorkDirectory.AgcpostlpfRepository().GetByBusinessUnits(businessUnits);
            var mapper = entity.OrderBy(o => o.Agcplocali).Select(s => _mapper.Map<ComboStringDto>(s));
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }

        public async Task<ServiceResponse<IEnumerable<ComboAgrupacionStringDto>>> GetComboEstadosSuministroAgrupacionAsync()
        {
            var entity = await _unitOfWorkNotificacion.EstadosSuministroRepository().GetAllAsync();
            var response = entity
                .OrderBy(o => o.Grupo)
                .GroupBy(g => g.Grupo)
                .Select(s => new ComboAgrupacionStringDto
                {
                    Agrupacion = s.Key.Trim(),
                    Detalle = s.Select(m => new ComboStringDto
                    {
                        Id = m.Codigo,
                        Descripcion = m.Descripcion
                    })
                }).OrderBy(o=> o.Agrupacion).AsEnumerable();
            return ServiceResponseFactory.CreateOkResponse(response);

        }

        public async Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboBusinessUnitAsync()
        {
            var businessUnits = await _unitOfWorkDirectory.BusinessUnitRepository().GetExcludingNullsAsync();
            var mapper = businessUnits.OrderBy(o=> o.Name).Select(s=> _mapper.Map<ComboStringDto>(s));
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }

        public async Task<ServiceResponse<IEnumerable<ComboGrupoCategoriasDto>>> GetComboGrupoCategoriasAsync()
        {
            var grupos = await _unitOfWorkNotificacion.ClienteCategoriasRepository()
                .GetComboGrupoCategoriasAsync();
            return ServiceResponseFactory.CreateOkResponse(grupos);
        }

        public async Task<ServiceResponse<IEnumerable<ClaseCampaniaResponse>>> GetAllAsync()
        {
            var response = await _unitOfWorkNotificacion.ClaseCampaniaRepository().GetAllAsync();
            var entities = response.Select(s => new ClaseCampaniaResponse
            {
                IdClaseCampania = s.IdClaseCampania,
                descripcion = s.descripcion
            });
            return ServiceResponseFactory.CreateOkResponse(entities);
        }

        public async Task<ServiceResponse<IEnumerable<ClaseCampaniaResponse>>> GetComboClasesCampaniasAsync()
        {
            try
            {
                var response = await _unitOfWorkNotificacion.ClaseCampaniaRepository().GetAllAsync();
                var entities = response.Select(s => new ClaseCampaniaResponse
                {
                    IdClaseCampania = s.IdClaseCampania,
                    descripcion = s.descripcion
                });
                return ServiceResponseFactory.CreateOkResponse(entities);
            }
            catch (Exception ec)
            { throw ec; }
            
        }
    }
}
