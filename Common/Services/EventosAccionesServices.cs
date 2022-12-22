using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using OfficeOpenXml;

namespace Common.Services
{
    public class EventosAccionesServices : IEventosAccionesServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWorkNotificacion;
        private readonly IMapper _mapper;

        public EventosAccionesServices(IUnitOfWorkNotificacion unitOfWorkNotificacion, IMapper mapper)
        {
            _unitOfWorkNotificacion = unitOfWorkNotificacion;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GridEventosAccionesResponse>> GridAsync(GridEventosAccionesRequest request)
        {
            var response = new GridEventosAccionesResponse
            {
                Grid = await _unitOfWorkNotificacion.EventosAccionesRepository().GridAsync(request),
                Cantidad = await _unitOfWorkNotificacion.EventosAccionesRepository().CounterAsync(request)
            };
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(GridEventosAccionesRequest request)
        {
            var excel = await _unitOfWorkNotificacion.EventosAccionesRepository().ExcelAsync(request);
            var response = new ExportFileResponse { Title = $"Reporte_Notificaciones_Digitales_{DateTime.Now:yyyyMMddhhmmss}.xlsx" };

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                package.Workbook.Properties.Title = $"Reporte Notificaciones Digitales {DateTime.Now.Ticks}";
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
        private void BuildExcel(ExcelWorksheet worksheet, IEnumerable<EventosAccionesDto> vencimientos, int row)
        {
            worksheet.Cells[row, 1].Value = "Razón";
            worksheet.Cells[row, 2].Value = "Código";
            worksheet.Cells[row, 3].Value = "Código Rechazo";
            worksheet.Cells[row, 4].Value = "Severidad";
            worksheet.Cells[row, 5].Value = "Resultante";
            worksheet.Cells[row, 6].Value = "Activado";
            row++;
            foreach (var item in vencimientos)
            {
                worksheet.Cells[row, 1].Value = item.Razon;
                worksheet.Cells[row, 2].Value = item.Codigo;
                worksheet.Cells[row, 3].Value = item.CodigoRechazo;
                worksheet.Cells[row, 4].Value = item.Severidad;
                worksheet.Cells[row, 5].Value = item.ResultanteRechazo;
                worksheet.Cells[row, 6].Value = item.Activo;
            }
        }

        public async Task<ServiceResponse<EventoAccionesFormularioDto>> GetAsync(long id)
        {
            var entity = await _unitOfWorkNotificacion.EventosAccionesRepository()
                .FindByAsync(f => f.IdConfiguracionEmail == id);
            var mapper = _mapper.Map<EventoAccionesFormularioDto>(entity);
            return ServiceResponseFactory.CreateOkResponse(mapper);
        }

        public async Task<ServiceResponse<bool>> PutAsync(long id, EventoAccionesFormularioDto item)
        {
            var entity = await _unitOfWorkNotificacion.EventosAccionesRepository()
                .FindByAsync(f => f.IdConfiguracionEmail == id);
            entity.AutorModificacion = item.AutorModificacion;
            entity.FechaModificacion = DateTime.Now;
            entity.IdEventoResultanteEmail = item.IdEventoResultante;
            entity.Activo = item.Activo;
            await _unitOfWorkNotificacion.EventosAccionesRepository().UpdateAsync(entity, f=> f.IdEventoResultanteEmail == id);
            await _unitOfWorkNotificacion.SaveChangeAsync();
            return ServiceResponseFactory.CreateOkResponse(true);
        }
    }
}
