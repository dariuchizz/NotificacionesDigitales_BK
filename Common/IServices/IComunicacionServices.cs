using System.Collections.Generic;
using Common.Model.Dto;
using System.Threading.Tasks;
using Common.Enums;
using System;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;

namespace Common.IServices
{
    public interface IComunicacionServices
    {
        Task<long> AddComunicacionAsync(ComunicacionDto cuenta);

        Task<bool> UpdateComunicacionAsync(ComunicacionDto cuenta);

        Task<ComunicacionDto> GetIdByMailgunAsync(string MessageId);

        Task<bool> UpdateComunicacionesAsync(List<ComunicacionDto> comunicacionesDto);

        Task<IEnumerable<ComunicacionDto>> GetErroresAsync();

        Task<ComunicacionDto> GetAsync(long id);

        Task<ComunicacionDto> GetAsync(long idComunicaion, Guid GUID);

        Task<IEnumerable<ComunicacionDto>> GetByTipoComunicacionIdRelacionadoAsync(
            ComunicacionType comunicacion, long idRleacionado);

        Task<ServiceResponse<ReporteEventosPorCuentaResponse>> ReportAsync(ReporteEventosPorCuentaRequest request);
        Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(ReporteEventosPorCuentaRequest request);
                
        Task<ComunicacionDto> GetRelEnvioAsync(long id);
    }
}
