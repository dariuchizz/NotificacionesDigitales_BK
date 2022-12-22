using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface ICampaniaServices
    {
        Task<CampaniaDto> GetAsync(long id);        

        Task<CampaniaDto> GetWithRelationshipsAsync(long id);
        
        Task<ServiceResponse<CampaniaResponse>> GridAsync(GridCampaniaRequest request);
        
        Task<ServiceResponse<CampaniaDto>> AddCampaignAsync(CampaniaDto campaniaDto, int userId);
        
        Task<ServiceResponse<CampaniaDto>> UpdateCampaignAsync(CampaniaDto campaniaDto, int userId);
        
        Task<CampaniaDto> UpdateAsync(CampaniaDto campaniaDto, int userId);
        
        Task<ServiceResponse<IEnumerable<ConsultarCampaniaDto>>> GetDatosCampaniaAsync(CampaniaDto campaniaDto);
        
        Task<ServiceResponse<bool>> UpdateActiveAsync(long idCampania, bool activated, int userId);
        
        Task<ServiceResponse<bool>> DuplicateAsync(long idCampania, int userId);
        
        Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(GridCampaniaRequest request);
        
        Task<ServiceResponse<ExportFileResponse>> DownloadExcelDemoAsync();

        Task<CampaniaDto> GetCampaniaHtmlAsync(long id);
    }
}
