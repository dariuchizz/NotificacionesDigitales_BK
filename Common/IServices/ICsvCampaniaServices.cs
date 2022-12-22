using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.Services;

namespace Common.IServices
{
    public interface ICsvCampaniaServices
    {
        Task<CsvCampaniaDto> GetFirstRowAsync(long idCampania);        
    }
}
