using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using OfficeOpenXml;

namespace Common.Services
{
    public class CobrabilidadServices: ICobrabilidadServices
    {
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        public CobrabilidadServices(IUnitOfWorkNotificacion unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse<CobrabilidadResponse>> ReportAsync(CobrabilidadRequest request)
        {
            var response = new CobrabilidadResponse
            {
                Grid = await _unitOfWork.CobrabilidadRepository().ReportAsync(request),
                Cantidad = await _unitOfWork.CobrabilidadRepository().ReportCountAsync(request)
            };
            return ServiceResponseFactory.CreateOkResponse(response);
        }
        public async Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(CobrabilidadRequest request)
        {
            var response = new ExportFileResponse { Title = $"Reporte_Notificaciones_Digitales_{DateTime.Now:yyyyMMddhhmmss}.xlsx" };
            var vencimientos = await _unitOfWork.CobrabilidadRepository().ExportExcelAsync(request);

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                package.Workbook.Properties.Title = $"Reporte Notificaciones Digitales {DateTime.Now.Ticks}";
                var worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Name = "Hoja 1";
                var row = 1;
                if (request.TipoComunicacion == 5)
                {
                    BuildExcelAvisoCorte(worksheet, vencimientos, row);
                }
                else
                {
                    BuildExcelDefault(worksheet, vencimientos, row);
                }
                package.Save();
            }

            stream.Position = 0;
            response.FileStream = stream.ToArray();
            return ServiceResponseFactory.CreateOkResponse(response);

        }

        private void BuildExcelDefault(ExcelWorksheet worksheet, IEnumerable<NotificacionesDigitalesHaedDto> vencimientos, int row)
        {
            worksheet.Cells[row, 1].Value = "Id Envío";
            worksheet.Cells[row, 2].Value = "Fecha Envío";
            worksheet.Cells[row, 3].Value = "Fecha Vto.";
            worksheet.Cells[row, 4].Value = "Notificaciones";
            worksheet.Cells[row, 5].Value = "Facturas";
            worksheet.Cells[row, 6].Value = "Total Notificado";
            worksheet.Cells[row, 7].Value = "Días hasta el Pago";
            worksheet.Cells[row, 8].Value = "Fecha de Pago";
            worksheet.Cells[row, 9].Value = "Facturas Cobradas";
            worksheet.Cells[row, 10].Value = "Porcentaje Cobrado";
            worksheet.Cells[row, 11].Value = "Total Recaudado";
            worksheet.Cells[row, 12].Value = "Porcentaje Recaudado";
            row++;
            foreach (var item in vencimientos)
            {

                worksheet.Cells[row, 1].Value = item.IdEnvio;
                worksheet.Cells[row, 2].Value = item.FechaEnvio.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 3].Value = item.FechaVencimiento.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 4].Value = item.CantidadNotificaciones;
                worksheet.Cells[row, 5].Value = item.CantidadFacturas;
                worksheet.Cells[row, 6].Value = item.TotalNotificado;
                if (item.Detalle.Any())
                {
                    foreach (var deta in item.Detalle)
                    {
                        worksheet.Cells[row, 7].Value = deta.CantidadDias;
                        worksheet.Cells[row, 8].Value = deta.FechaPago.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 9].Value = deta.CantidadFacturas;
                        worksheet.Cells[row, 10].Value = $"{deta.PorcentajeCobrado} %";
                        worksheet.Cells[row, 11].Value = $"$ {deta.TotalRecaudado}";
                        worksheet.Cells[row, 12].Value = $"{deta.PorcentajeRecaudado} %";
                        row++;
                    }
                    worksheet.Cells[row, 7].Value = "Total";
                    worksheet.Cells[row, 9].Value = item.Detalle.Sum(s => s.CantidadFacturas);
                    worksheet.Cells[row, 10].Value = $"{item.Detalle.Sum(s => s.PorcentajeCobrado)} %";
                    worksheet.Cells[row, 11].Value = $"$ {item.Detalle.Sum(s => s.TotalRecaudado)}";
                    worksheet.Cells[row, 12].Value = $"{item.Detalle.Sum(s => s.PorcentajeRecaudado)} %";
                    row++;
                }
                else
                {
                    row++;
                }
            }

        }
        private void BuildExcelAvisoCorte(ExcelWorksheet worksheet, IEnumerable<NotificacionesDigitalesHaedDto> vencimientos, int row)
        {
            worksheet.Cells[row, 1].Value = "Id Envío";
            worksheet.Cells[row, 2].Value = "Fecha Envío";
            worksheet.Cells[row, 3].Value = "Notificaciones";
            worksheet.Cells[row, 4].Value = "Cantidad";
            worksheet.Cells[row, 5].Value = "Total Notificado";
            worksheet.Cells[row, 6].Value = "Días hasta el Pago";
            worksheet.Cells[row, 7].Value = "Fecha de Pago";
            worksheet.Cells[row, 8].Value = "Comp. Pagados";
            worksheet.Cells[row, 9].Value = "Porcentaje Cobrado";
            worksheet.Cells[row, 10].Value = "Total Recaudado";
            worksheet.Cells[row, 11].Value = "Porcentaje Recaudado";
            row++;
            foreach (var item in vencimientos)
            {

                worksheet.Cells[row, 1].Value = item.IdEnvio;
                worksheet.Cells[row, 2].Value = item.FechaEnvio.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 3].Value = item.CantidadNotificaciones;
                worksheet.Cells[row, 4].Value = item.CantidadFacturas;
                worksheet.Cells[row, 5].Value = item.TotalNotificado;
                if (item.Detalle.Any())
                {
                    foreach (var deta in item.Detalle)
                    {
                        worksheet.Cells[row, 6].Value = deta.CantidadDias;
                        worksheet.Cells[row, 7].Value = deta.FechaPago.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 8].Value = deta.CantidadFacturas;
                        worksheet.Cells[row, 9].Value = $"{deta.PorcentajeCobrado} %";
                        worksheet.Cells[row, 10].Value = $"$ {deta.TotalRecaudado}";
                        worksheet.Cells[row, 11].Value = $"{deta.PorcentajeRecaudado} %";
                        row++;
                    }
                    worksheet.Cells[row, 6].Value = "Total";
                    worksheet.Cells[row, 8].Value = item.Detalle.Sum(s => s.CantidadFacturas);
                    worksheet.Cells[row, 9].Value = $"{item.Detalle.Sum(s => s.PorcentajeCobrado)} %";
                    worksheet.Cells[row, 10].Value = $"$ {item.Detalle.Sum(s => s.TotalRecaudado)}";
                    worksheet.Cells[row, 11].Value = $"{item.Detalle.Sum(s => s.PorcentajeRecaudado)} %";
                    row++;
                }
                else
                {
                    row++;
                }
            }

        }

    }
}
