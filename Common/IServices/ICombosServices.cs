using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface ICombosServices
    {
        Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboEventosResultantesEmailsAsync();
        Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboCanalesAsync();
        Task<ServiceResponse<IEnumerable<ComboLongDto>>> GetComboEstadosCampaniaAsync();
        Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboEstadosSuministroAsync();
        Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboCategoriasSuministroAsync();
        Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboLocalidadesByBusinessUnitAsync(List<string> businessUnits);
        Task<ServiceResponse<IEnumerable<ComboAgrupacionStringDto>>> GetComboEstadosSuministroAgrupacionAsync();
        Task<ServiceResponse<IEnumerable<ComboStringDto>>> GetComboBusinessUnitAsync();
        Task<ServiceResponse<IEnumerable<ComboGrupoCategoriasDto>>> GetComboGrupoCategoriasAsync();
        Task<ServiceResponse<IEnumerable<ClaseCampaniaResponse>>> GetComboClasesCampaniasAsync();
    }
}
