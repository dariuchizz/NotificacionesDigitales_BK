using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using System.Threading.Tasks;
using Common.Enums;
using System;
using System.IO;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using OfficeOpenXml;

namespace Common.Services
{
    public class ComunicacionServices : IComunicacionServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IMapper _mapper;

        public ComunicacionServices(IUnitOfWorkNotificacion unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> AddComunicacionAsync(ComunicacionDto dto)
        {
            var comunicacion = _mapper.Map<Comunicacion>(dto);
            await _unitOfWork.ComunicacionRepository().AddAsync(comunicacion);
            await _unitOfWork.SaveChangeAsync();
            return comunicacion.IdComunicacion;
        }

        public async Task<bool> UpdateComunicacionAsync(ComunicacionDto dto)
        {
            var comunicacion = _mapper.Map<Comunicacion>(dto);
            await _unitOfWork.ComunicacionRepository().UpdateAsync(comunicacion, comunicacion => comunicacion.IdComunicacion);
            var result = await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<bool> UpdateComunicacionesAsync(List<ComunicacionDto> comunicacionesDto)
        {
            var comunicaciones = new List<Comunicacion>();
            foreach (var item in comunicacionesDto)
            {
                var com = await _unitOfWork.ComunicacionRepository()
                    .FindByAsync(f => f.IdComunicacion == item.IdComunicacion);
                com.FechaProceso = item.FechaProceso;
                com.Enviado = item.Enviado;
                com.IdExterno = item.IdExterno;
                com.Message = item.Message;
                comunicaciones.Add(com);
            }

            await _unitOfWork.ComunicacionRepository().UpdateRangeAsync(comunicaciones);
            var result = await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<ComunicacionDto> GetIdByMailgunAsync(string MessageId)
        {
            return await _unitOfWork.ComunicacionRepository().GetIdByMailgunAsync(MessageId);
        }

        public async Task<IEnumerable<ComunicacionDto>> GetErroresAsync()
        {
            var temporal = await _unitOfWork.ComunicacionRepository().GetErroresAsync();
            var comunicaciones = temporal.Select(s => _mapper.Map<ComunicacionDto>(s));
            return comunicaciones;
        }

        public async Task<ComunicacionDto> GetAsync(long id)
        {
            var com = await _unitOfWork.ComunicacionRepository().FindByAsync(f => f.IdComunicacion == id);
            var comunicacion = _mapper.Map<ComunicacionDto>(com);
            return comunicacion;
        }

        public async Task<ComunicacionDto> GetRelEnvioAsync(long id)
        {
            var com = await _unitOfWork.ComunicacionRepository().FindByAsync(f => f.IdComunicacion == id, c => c.Envio);
            var comunicacion = _mapper.Map<ComunicacionDto>(com);
            return comunicacion;
        }

        public async Task<ComunicacionDto> GetAsync(long idComunicacion, Guid GUID)
        {
            var com = await _unitOfWork.ComunicacionRepository().FindByAsync(f => f.IdComunicacion == idComunicacion && f.GUID == GUID);
            var comunicacion = _mapper.Map<ComunicacionDto>(com);
            return comunicacion;
        }

        public async Task<ComunicacionDto> GetByIdRelacionadoAsync(long id)
        {
            var com = await _unitOfWork.ComunicacionRepository().FindByAsync(f => f.IdComunicacion == id);
            var comunicacion = _mapper.Map<ComunicacionDto>(com);
            return comunicacion;
        }

        public async Task<IEnumerable<ComunicacionDto>> GetByTipoComunicacionIdRelacionadoAsync(
            ComunicacionType comunicacion, long idRleacionado)
        {
            var coms = await _unitOfWork.ComunicacionRepository()
                .GetByTipoComunicacionIdRelacionadoAsync(comunicacion, idRleacionado);
            var response = coms.Select(s => _mapper.Map<ComunicacionDto>(s));
            return response;
        }

        public async Task<ServiceResponse<ReporteEventosPorCuentaResponse>> ReportAsync(ReporteEventosPorCuentaRequest request)
        {
            await BuildRestRequest(request);
            var response = new ReporteEventosPorCuentaResponse
            {
                Cantidad = await _unitOfWork.ComunicacionRepository().ReportCounterAsync(request),
                Grid = await _unitOfWork.ComunicacionRepository().ReportAsync(request)
            };
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(ReporteEventosPorCuentaRequest request)
        {
            await BuildRestRequest(request);
            var excel = await _unitOfWork.ComunicacionRepository().ExportExcelAsync(request);
            var response = new ExportFileResponse { Title = $"Reporte_Eventos_{DateTime.Now:yyyyMMddhhmmss}.xlsx" };

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                package.Workbook.Properties.Title = $"Reporte Eventos {DateTime.Now.Ticks}";
                var worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Name = "Hoja 1";
                var row = 1;
                BuildExcel(worksheet, excel, row);
                package.Save();
            }

            stream.Position = 0;
            response.FileStream = stream.ToArray();
            return ServiceResponseFactory.CreateOkResponse(response);

        }

        private async Task BuildRestRequest(ReporteEventosPorCuentaRequest request)
        {
            var suministro = await _unitOfWork.SuministroRepository().FindByAsync(f=> f.CuentaUnificada.ToString() == request.CuentaUnificada);
            if (suministro != null)
            {
                request.IdSuministro = suministro.IdSuministro;
            }
        }
        private void BuildExcel(ExcelWorksheet worksheet, IEnumerable<ReporteEventosPorCuentaDto> items, int row)
        {
            worksheet.Cells[row, 1].Value = "Fecha";
            worksheet.Cells[row, 2].Value = "Evento";
            worksheet.Cells[row, 3].Value = "Razón";
            worksheet.Cells[row, 4].Value = "Código";
            worksheet.Cells[row, 5].Value = "Código Rechazo";
            worksheet.Cells[row, 6].Value = "Severidad";
            worksheet.Cells[row, 7].Value = "Mensaje Error";
            row++;
            //foreach (var item in items)
            //{
            //    worksheet.Cells[row, 1].Value = item.Fecha.Value.ToString("dd/MM/yyyy");
            //    worksheet.Cells[row, 2].Value = item.Evento;
            //    worksheet.Cells[row, 3].Value = item.Reason;
            //    worksheet.Cells[row, 4].Value = item.Code;
            //    worksheet.Cells[row, 5].Value = item.BouncedCode;
            //    worksheet.Cells[row, 6].Value = item.Severity;
            //    worksheet.Cells[row, 7].Value = item.MessageError;
            //}
        }        
    }
}