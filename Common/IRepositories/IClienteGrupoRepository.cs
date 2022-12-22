using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;

namespace Common.IRepositories
{
    public interface IClienteGrupoRepository: IGenericRepository<ClienteGrupoCategoria>
    {
        Task<IEnumerable<ComboGrupoCategoriasDto>> GetComboGrupoCategoriasAsync();
        Task<List<string>> GetGrupoCategoriasByCategoriasAsync(List<string> categorias);

        Task<List<string>> GetGrupoCategoriasByCategoriasAndGranClienteAsync(List<string> categorias, bool granCliente);
        Task<List<string>> BuildCodesCategoriasByGroupAsync(List<string> codes);
    }
}
