using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.IServices;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;
using Common.Model.Request;
using Common.Model.Response;
using Common.Model.Services;
using OfficeOpenXml;

namespace Common.Services
{
    public class CampaniaServices : ICampaniaServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkNotificacion _unitOfWork;
        private readonly IUnitOfWorkDirectory _unitOfWorkDirectory;

        public CampaniaServices(IMapper mapper, IUnitOfWorkNotificacion unitOfWork, IUnitOfWorkDirectory unitOfWorkDirectory)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _unitOfWorkDirectory = unitOfWorkDirectory;
        }
        public async Task<CampaniaDto> GetAsync(long id)
        {
            var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == id,
                c => c.TipoCampania,
                c => c.EstadoCampania);
            var campaniaDto = _mapper.Map<CampaniaDto>(campania);
            return campaniaDto;
        }

        public async Task<CampaniaDto> GetWithRelationshipsAsync(long id)
        {
            var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == id,
                c => c.TipoCampania,
                c => c.EstadoCampania,
                c => c.ParametrosCampanias,
                //c => c.CsvCampanias,
                c => c.CampaniasHtmlVariables);
            var campaniaDto = _mapper.Map<CampaniaDto>(campania);
            if (campaniaDto.TipoSeleccion == 1 || campaniaDto.TipoSeleccion == 4)
            {
                await GetParametrosAsync(campaniaDto);
            }
            /*if (campaniaDto.TipoSeleccion == 3)
            {
                await GetCsvAsync(campaniaDto);
            }*/
            return campaniaDto;
        }

        private async Task GetCsvAsync(CampaniaDto campaniaDto)
        {
            var csvCampanias = await _unitOfWork.CsvCampaniaRepository()
                .SearchByAsync(s => s.IdCampania == campaniaDto.IdCampania);
            campaniaDto.CsvCampanias = csvCampanias.Select(s => _mapper.Map<CsvCampaniaDto>(s));
            if (csvCampanias != null)
            {
                campaniaDto.CantidadRegistrosCsv = csvCampanias.Count();
            }
        }

        private async Task GetParametrosAsync(CampaniaDto campaniaDto)
        {
            if (campaniaDto.ParametrosCampania != null)
            {
                campaniaDto.ParametrosCampania.UnidadesNegocio =
                    await _unitOfWorkDirectory.AgcpostlpfRepository()
                        .GetBusinessUnitsByZipCodeAsync(campaniaDto.ParametrosCampania.Localidades);
                campaniaDto.ParametrosCampania.Categorias = await _unitOfWork.ClienteCategoriasRepository()
                    .GetGrupoCategoriasByCategoriasAsync(campaniaDto.ParametrosCampania.CategoriasDetalle);

                campaniaDto.ParametrosCampania.CategoriasDetalle = await _unitOfWork.ClienteCategoriasRepository()
                    .BuildCodesCategoriasByGroupAsync(campaniaDto.ParametrosCampania.CategoriasDetalle);
                if (campaniaDto.ParametrosCampania.GranCliente == 0)
                {
                    var granCliente = campaniaDto.ParametrosCampania.Categorias.Find(f => f.Equals("SGP Con Contrato"));
                    if (!string.IsNullOrEmpty(granCliente))
                    {
                        campaniaDto.ParametrosCampania.Categorias.Remove(granCliente);
                    }

                    var granClienteDetalle =
                        campaniaDto.ParametrosCampania.CategoriasDetalle.Find(f => f.Trim().Equals("3-SGP"));
                    if (!string.IsNullOrEmpty(granClienteDetalle))
                    {
                        campaniaDto.ParametrosCampania.CategoriasDetalle.Remove(granClienteDetalle);
                    }
                }

                if (campaniaDto.ParametrosCampania.GranCliente == 1)
                {
                    var granCliente = campaniaDto.ParametrosCampania.Categorias.Find(f => f.Equals("SGP Sin Contrato"));
                    if (!string.IsNullOrEmpty(granCliente))
                    {
                        campaniaDto.ParametrosCampania.Categorias.Remove(granCliente);
                    }

                    var granClienteDetalle =
                        campaniaDto.ParametrosCampania.CategoriasDetalle.Find(f => f.Trim().Equals("2-SGP"));
                    if (!string.IsNullOrEmpty(granClienteDetalle))
                    {
                        campaniaDto.ParametrosCampania.CategoriasDetalle.Remove(granClienteDetalle);
                    }
                }
            }
        }

        public async Task<CampaniaDto> UpdateAsync(CampaniaDto campaniaDto, int userId)
        {
            var estadoCampania = await _unitOfWork.EstadoCampaniasRepository()
                .FindByAsync(f => f.IdEstadoCampania == campaniaDto.IdEstadoCampania);
            campaniaDto.EstadoCampania = _mapper.Map<EstadoCampaniaDto>(estadoCampania);
            var campania = _mapper.Map<Campania>(campaniaDto);
            campania.AutorModificacion = userId;
            campania.FechaModificacion = DateTime.Now;
            await _unitOfWork.CampaniaRepository().UpdateAsync(campania, campania1 => campania1.IdCampania);
            await _unitOfWork.SaveChangeAsync();
            return campaniaDto;
        }

        public async Task<ServiceResponse<CampaniaResponse>> GridAsync(GridCampaniaRequest request)
        {
            var grid = new CampaniaResponse
            {
                Grid = await _unitOfWork.CampaniaRepository().GridAsync(request),
                Cantidad = await _unitOfWork.CampaniaRepository().GridCounterAsync(request)
            };
            return ServiceResponseFactory.CreateOkResponse(grid);
        }

        public async Task<ServiceResponse<CampaniaDto>> AddCampaignAsync(CampaniaDto campaniaDto, int userId)
        {
            var campaign = new Campania
            {
                Activo = true,
                Asunto = campaniaDto.Asunto,
                Descripcion = campaniaDto.Descripcion,
                FechaCreacion = DateTime.Now,
                IdCampania = campaniaDto.IdCampania,
                IdEstadoCampania = 1,
                Template = campaniaDto.TemplateHtml,
                Tag = campaniaDto.Tag,
                TopeLectura = campaniaDto.TopeLectura,
                IdTipoCampania = campaniaDto.TipoSeleccion,
                IdClaseCampania = campaniaDto.ClaseCampaniaSeleccion,
                Autor = userId,
                IdCanalCampania = Convert.ToInt32(campaniaDto.CanalCampaniaSeleccion),
                TextoSMS = campaniaDto.TextoSMS
            };
            var newEntity = await _unitOfWork.CampaniaRepository().AddWithReturnAsync(campaign);
            await _unitOfWork.SaveChangeAsync();
            campaniaDto = _mapper.Map<CampaniaDto>(newEntity);
            return ServiceResponseFactory.CreateOkResponse(campaniaDto);
        }

        public async Task<ServiceResponse<CampaniaDto>> UpdateCampaignAsync(CampaniaDto campaniaDto, int userId)
        {
            var campania = await MapperCampaingToUpdateAsync(campaniaDto, userId);
            switch (campaniaDto.ActualStep)
            {
                case 2:
                    await UpdateStep2Async(campaniaDto, campania);
                    break;
                case 3:
                    await UpdateVariablesHtmlAsync(campaniaDto, campania);
                    break;
            }

            //if (campaniaDto.ParametrosCampania != null)
            //{
            //    if (campania.ParametrosCampanias.Count > 0)
            //    {
            //        campaniaDto.ParametrosCampania.UnidadesNegocio =
            //            await _unitOfWorkDirectory.AgcpostlpfRepository()
            //                .GetBusinessUnitsByZipCodeAsync(campaniaDto.ParametrosCampania.Localidades);
            //    }                
            //}
            try
            {
                await _unitOfWork.CampaniaRepository().UpdateAsync(campania, c => c.IdCampania);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception ec) { throw ec; }

            var parametros = await _unitOfWork.ParametrosCampaniasRepository().FindByAsync(f => f.IdCampania == campaniaDto.IdCampania);
            if (parametros != null)
            {
                List<string> listGrupoCat = new List<string>(parametros.Categorias.ToString().Split(','));
                var grupoCategoriaSeleccionadas = await _unitOfWork.ClienteCategoriasRepository().GetGrupoCategoriasByCategoriasAndGranClienteAsync(listGrupoCat, Convert.ToBoolean(parametros.GranCliente));

                campaniaDto.ParametrosCampania = new ParametrosCampaniasDto
                {
                    IdParametro = parametros.IdParametro, //campania.ParametrosCampanias.First().IdParametro,
                    Categorias = grupoCategoriaSeleccionadas,//new List<string>(parametros.Categorias.ToString().Split(',')), //new List<string>(campania.ParametrosCampanias.First().Categorias.ToString().Split(',')),
                    Estados = new List<string>(parametros.Estados.ToString().Split(',')), //new List<string>(campania.ParametrosCampanias.First().Estados.ToString().Split(',')),
                    Localidades = new List<string>(parametros.Localidades.ToString().Split(',')), //new List<string>(campania.ParametrosCampanias.First().Localidades.ToString().Split(',')),
                    GranCliente = parametros.GranCliente == null ? -1 : Convert.ToInt32(parametros.GranCliente), //Convert.ToInt32(campania.ParametrosCampanias.First().GranCliente),
                    EnteOficial = parametros.EnteOficial == null ? -1 : Convert.ToInt32(parametros.EnteOficial), //Convert.ToInt32(campania.ParametrosCampanias.First().EnteOficial),
                    TieneDebitoAutomatico = parametros.TieneDebitoAutomatico == null ? -1 : Convert.ToInt32(parametros.TieneDebitoAutomatico), //Convert.ToInt32(campania.ParametrosCampanias.First().TieneDebitoAutomatico),
                    IdMotivoBaja = Convert.ToInt32(parametros.IdMotivoBaja), //Convert.ToInt32(campania.ParametrosCampanias.First().IdMotivoBaja),
                    TieneNotificacionDigital = parametros.TieneNotificacionDigital == null ? -1 : Convert.ToInt32(parametros.TieneNotificacionDigital), //Convert.ToInt32(campania.ParametrosCampanias.First().TieneNotificacionDigital)
                    CategoriasDetalle = campaniaDto.ParametrosCampania.CategoriasDetalle
                };

                campaniaDto.ParametrosCampania.UnidadesNegocio =
                    await _unitOfWorkDirectory.AgcpostlpfRepository().GetBusinessUnitsByZipCodeAsync(parametros.Localidades.ToString().Split(',').ToList());
            }
            else {
                campaniaDto.ParametrosCampania = null;
            }
           
            if (campaniaDto.TipoSeleccion == 1 && campaniaDto.ActualStep == 2)
            {
                var addedParameter = await _unitOfWork.ParametrosCampaniasRepository()
                    .SearchByAsync(s => s.IdCampania == campaniaDto.IdCampania);
                campaniaDto.ParametrosCampania.IdParametro = addedParameter.Max(m => m.IdParametro);
            }
            campaniaDto.CantidadEmails = campania.CantidadEmails;
            campaniaDto.CantidadRegistrosCsv = campania.CantidadEmails;
            return ServiceResponseFactory.CreateOkResponse(campaniaDto);
        }

        private async Task<Campania> MapperCampaingToUpdateAsync(CampaniaDto campaniaDto, int userId)
        {            
            var campania = await _unitOfWork.CampaniaRepository()
                    .FindByAsync(f => f.IdCampania == campaniaDto.IdCampania, t => t.TipoCampania);
            campania.Activo = true;
            campania.Asunto = campaniaDto.Asunto;
            campania.AutorModificacion = campaniaDto.AutorModificacion;
            campania.Descripcion = campaniaDto.Descripcion;
            campania.FechaModificacion = DateTime.Now;
            campania.Tag = campaniaDto.Tag;
            campania.Template = campaniaDto.TemplateName?.ToLowerInvariant().Replace(" ", "_");
            campania.TopeLectura = campaniaDto.TopeLectura;
            campania.AutorModificacion = userId;
            campania.IdClaseCampania = campaniaDto.ClaseCampaniaSeleccion;
            campania.CantidadEmails = campania.CantidadEmails;            
            campania.TextoSMS = campaniaDto.TextoSMS;

            if ((campania.IdTipoCampania != campaniaDto.TipoSeleccion) || (campania.IdCanalCampania != campaniaDto.CanalCampaniaSeleccion))            
            {
                //En el caso que haya un cambio de CANAL se elimina los registros adjuntos en el CSV de la tabla CsvCampania                
                var segmentos = await _unitOfWork.CsvCampaniaRepository().SearchByAsync(s => s.IdCampania == campaniaDto.IdCampania);
                if (segmentos != null && segmentos.Count() > 0)
                {
                    foreach (var item in segmentos)
                    {
                        await _unitOfWork.CsvCampaniaRepository().DeleteAsync(item);
                        campania.CsvCampanias.Remove(item);
                    }
                    await _unitOfWork.SaveChangeAsync();                    
                }

                //if (campania.ParametrosCampanias.Any())
                //{
                //    var param = await _unitOfWork.ParametrosCampaniasRepository().SearchByAsync(f => f.IdCampania == campaniaDto.IdCampania);
                //    await _unitOfWork.ParametrosCampaniasRepository().DeleteAsync(param.First());
                //    await _unitOfWork.SaveChangeAsync();
                //    campania.ParametrosCampanias.Remove(param.First());
                //}

                var param = await _unitOfWork.ParametrosCampaniasRepository().FindByAsync(x => x.IdCampania == campania.IdCampania);
                if (param != null)
                {
                    await _unitOfWork.ParametrosCampaniasRepository().DeleteAsync(param);
                    await _unitOfWork.SaveChangeAsync();
                    
                    //campania.ParametrosCampanias.Add(param);
                }

                campania.IdCanalCampania = Convert.ToInt32(campaniaDto.CanalCampaniaSeleccion);
                campania.IdTipoCampania = campaniaDto.TipoSeleccion;
                campania.CantidadEmails = 0;
                campania.CantidadSuministros = 0;
                campaniaDto.PathFullCsv = "";
                campania.Template = "";                
                campania.TipoCampania = await _unitOfWork.TipoCampaniaRepository()
                    .FindByAsync(f => f.IdTipoCampania == campaniaDto.TipoSeleccion);
                             
            }
            //await UpdateStep2Async(campaniaDto, campania);
            return campania;
        }

        private async Task UpdateVariablesHtmlAsync(CampaniaDto campaniaDto, Campania campania)
        {
            var variable = await _unitOfWork.CampaniasHtmlVariableRepository()
                .FindByAsync(f => f.IdCampania == campaniaDto.IdCampania);
            if (variable == null)
            {
                campania.CampaniasHtmlVariables = new List<CampaniasHtmlVariable>
                {
                    new CampaniasHtmlVariable
                    {
                        IdCampania = campaniaDto.IdCampania,
                        Nombre = campaniaDto.CampaniasHtmlVariableDto.Nombre,
                        NombreApellido = campaniaDto.CampaniasHtmlVariableDto.NombreApellido,
                        Apellido = campaniaDto.CampaniasHtmlVariableDto.Apellido,
                        Domicilio = campaniaDto.CampaniasHtmlVariableDto.Domicilio,
                        IdCampaniaHtmlVariable = 0,
                        Html = campaniaDto.TemplateHtml,
                    }
                };
            }
            else
            {
                variable.Nombre = campaniaDto.CampaniasHtmlVariableDto.Nombre;
                variable.Apellido = campaniaDto.CampaniasHtmlVariableDto.Apellido;
                variable.NombreApellido = campaniaDto.CampaniasHtmlVariableDto.NombreApellido;
                variable.Domicilio = campaniaDto.CampaniasHtmlVariableDto.Domicilio;
                variable.Html = campaniaDto.TemplateHtml;
                await _unitOfWork.CampaniasHtmlVariableRepository().UpdateAsync(variable, f => f.IdCampania);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        private async Task UpdateStep2Async(CampaniaDto campaniaDto, Campania campania)
        {
            var segmentos = await _unitOfWork.CsvCampaniaRepository()
                .SearchByAsync(s => s.IdCampania == campaniaDto.IdCampania);
            var parametros = await _unitOfWork.ParametrosCampaniasRepository()
                .FindByAsync(f => f.IdCampania == campaniaDto.IdCampania);

            if (campaniaDto.TipoSeleccion == 3 || campaniaDto.TipoSeleccion == 5)
            {
                //proceso CSV importado y lo cargo en la lista CsvCampanias
                if (!String.IsNullOrEmpty(campaniaDto.PathFullCsv))
                {
                    try
                    {
                        //leer archivo CSV y cargar en una lista
                        List<CsvCampaniaDto> list_campanias = new List<CsvCampaniaDto>();
                        list_campanias = ReadCsvCampaniaToList(campaniaDto.PathFullCsv, campaniaDto.CanalCampaniaSeleccion);
                        if (list_campanias.Count > 0)
                        {
                            campaniaDto.CsvCampanias = list_campanias;
                        }
                    }
                    catch (Exception ec) { throw ec; }
                }
                else {
                    if (segmentos.Count() > 0)
                    {
                        List<CsvCampaniaDto> list_campanias = new List<CsvCampaniaDto>();
                        campaniaDto.CsvCampanias = segmentos.Select(x => new CsvCampaniaDto {  
                            IdCsvCampania = x.IdCsvCampania,                           
                            IdCanal = x.IdCanal,
                            Dato = x.Dato,
                            Nombre = x.Nombre,
                            Apellido = x.Apellido,
                            NombreApellido = x.NombreApellido,
                            Domicilio = x.Domicilio,
                            Secuencia = x.Secuencia
                        });
                    }
                }

                if (campaniaDto.CsvCampanias != null && campaniaDto.CsvCampanias.Any())
                {
                    if (campaniaDto.HasChangeStep2)
                    {
                        if (segmentos.Any())
                        {
                            foreach (var item in segmentos)
                            {
                                await _unitOfWork.CsvCampaniaRepository().DeleteAsync(item);
                            }
                            await _unitOfWork.SaveChangeAsync();
                        }

                        var contador = 0;
                        foreach (var item in campaniaDto.CsvCampanias)
                        {
                            contador++;
                            item.Secuencia = contador;
                        }
                        campania.CsvCampanias = campaniaDto.CsvCampanias?.Select(s => _mapper.Map<CsvCampania>(s)).ToList();
                        campania.CantidadEmails = campaniaDto.CsvCampanias.LongCount();
                        campaniaDto.CantidadRegistrosCsv = campaniaDto.CsvCampanias.LongCount();
                    }
                    else
                    {
                        campania.CsvCampanias = segmentos.ToList();
                        campania.CantidadEmails = segmentos.LongCount(c => c.IdCanal == 1);
                        campaniaDto.CantidadRegistrosCsv = campania.CantidadEmails;
                    }
                }
            }
            else
            {
                if (segmentos.Any())
                {
                    foreach (var item in segmentos)
                    {
                        await _unitOfWork.CsvCampaniaRepository().DeleteAsync(item);
                    }
                    campaniaDto.CsvCampanias = new List<CsvCampaniaDto>();
                    campaniaDto.CantidadRegistrosCsv = 0;
                    campania.CantidadEmails = 0;
                }

                if (parametros == null)
                {
                    //await _unitOfWork.ParametrosCampaniasRepository().DeleteAsync(parametros);
                    //await _unitOfWork.SaveChangeAsync();
                    try
                    {
                        campania.ParametrosCampanias = new List<ParametrosCampania>
                        {
                            new ParametrosCampania
                            {
                                IdMotivoBaja = 0,
                                GranCliente = NumericToBoolean(campaniaDto.ParametrosCampania.GranCliente),
                                TieneNotificacionDigital =
                                    NumericToBoolean(campaniaDto.ParametrosCampania.TieneNotificacionDigital),
                                EnteOficial = NumericToBoolean(campaniaDto.ParametrosCampania.EnteOficial),
                                Categorias = campaniaDto.ParametrosCampania.CategoriasDetalle.Any()
                                    ? string.Join(',',
                                        campaniaDto.ParametrosCampania.CategoriasDetalle.Select(s => s.Split('-')[1]).Distinct())
                                    : "",
                                IdParametro = campaniaDto.ParametrosCampania.IdParametro,
                                Estados = campaniaDto.ParametrosCampania.Estados.Any()
                                    ? string.Join(',', campaniaDto.ParametrosCampania.Estados)
                                    : "",
                                TieneDebitoAutomatico = NumericToBoolean(campaniaDto.ParametrosCampania.TieneDebitoAutomatico),
                                Localidades = campaniaDto.ParametrosCampania.Localidades.Any()
                                    ? string.Join(',', campaniaDto.ParametrosCampania.Localidades)
                                    : ""
                            }
                        };
                    }
                    catch (Exception ex) { throw ex; }

                }
                else
                {
                    var paramDto = campaniaDto.ParametrosCampania;
                    parametros.IdCampania = campaniaDto.IdCampania;
                    parametros.TieneNotificacionDigital = NumericToBoolean(paramDto.TieneNotificacionDigital);
                    parametros.GranCliente = NumericToBoolean(paramDto.GranCliente);
                    parametros.TieneDebitoAutomatico = NumericToBoolean(paramDto.TieneDebitoAutomatico);
                    parametros.IdMotivoBaja = paramDto.IdMotivoBaja;
                    if (paramDto.CategoriasDetalle != null)
                    {
                        parametros.Categorias = string.Join(',', paramDto.CategoriasDetalle.Select(s => s.Split('-')[1]).Distinct());
                    }
                    else {
                        parametros.Categorias = "";
                    }                    
                    parametros.Localidades = paramDto.Localidades.Any()
                        ? string.Join(',', paramDto.Localidades)
                        : "";
                    parametros.Estados = paramDto.Estados.Any()
                        ? string.Join(',', paramDto.Estados)
                        : "";
                    //parametros.IdParametro = paramDto.IdParametro = Convert.ToBoolean(0) ? parametros.IdParametro : paramDto.IdParametro;
                    parametros.EnteOficial = NumericToBoolean(paramDto.EnteOficial);
                    await _unitOfWork.ParametrosCampaniasRepository().UpdateAsync(parametros, f => f.IdParametro);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
        }       

        private List<CsvCampaniaDto> ReadCsvCampaniaToList(string pathFullCsv, int CanalCampaniaSeleccion)
        {
            return File.ReadAllLines(pathFullCsv)
                                         .Skip(1)
                                         .Select(v => CsvCampaniaDto.FromCsv(v, CanalCampaniaSeleccion))
                                         .ToList();
        }

        public async Task<ServiceResponse<IEnumerable<ConsultarCampaniaDto>>> GetDatosCampaniaAsync(CampaniaDto campaniaDto)
        {
            var response = await _unitOfWork.StoreRepository()
                .GetDatosCampaniaAsync(campaniaDto.TipoCampania.StoreConsultar, campaniaDto.IdCampania);
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        public async Task<ServiceResponse<bool>> UpdateActiveAsync(long idCampania, bool activated, int userId)
        {
            var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == idCampania);
            campania.Activo = activated;
            campania.FechaModificacion = DateTime.Now;
            campania.AutorModificacion = userId;
            await _unitOfWork.CampaniaRepository().UpdateAsync(campania, a => a.IdCampania);
            await _unitOfWork.SaveChangeAsync();
            return ServiceResponseFactory.CreateOkResponse(true);
        }

        public async Task<ServiceResponse<bool>> DuplicateAsync(long idCampania, int userId)
        {
            var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == idCampania,
                t => t.TipoCampania, p => p.ParametrosCampanias, c => c.CsvCampanias, c => c.CampaniasHtmlVariables);

            var campaniaNew = _mapper.Map<Campania>(campania);
            campaniaNew.IdEstadoCampania = 1;
            campaniaNew.FechaCreacion = DateTime.Now;
            campaniaNew.CantidadEmails = 0;
            campaniaNew.CantidadSuministros = 0;
            campaniaNew.CantidadSuministros = 0;
            campaniaNew.IdScheduleGenerar = "";
            campaniaNew.IdScheduleObtener= "";
            campaniaNew.FechaPlanificado = null;
            campaniaNew.Autor = userId;
            await _unitOfWork.CampaniaRepository().AddAsync(campaniaNew);
            await _unitOfWork.SaveChangeAsync();
            return ServiceResponseFactory.CreateOkResponse(true);
        }

        public async Task<ServiceResponse<ExportFileResponse>> ExportExcelAsync(GridCampaniaRequest request)
        {
            var response = new ExportFileResponse { Title = $"Reporte_Notificaciones_Digitales_{DateTime.Now:yyyyMMddhhmmss}.xlsx" };
            var data = await _unitOfWork.CampaniaRepository().ExcelAsync(request);

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                package.Workbook.Properties.Title = $"Reporte Notificaciones Digitales {DateTime.Now.Ticks}";
                var worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Name = "Hoja 1";
                var row = 1;
                worksheet.Cells[row, 1].Value = "Nombre";
                worksheet.Cells[row, 2].Value = "Clase Campania";
                worksheet.Cells[row, 3].Value = "Fecha Creación";
                worksheet.Cells[row, 4].Value = "Fecha Planificada";
                worksheet.Cells[row, 5].Value = "Estado";
                worksheet.Cells[row, 6].Value = "Template";
                worksheet.Cells[row, 7].Value = "Asunto";
                worksheet.Cells[row, 8].Value = "Cantidad Contactos";
                worksheet.Cells[row, 9].Value = "Cantidad Suministros";
                row++;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Descripcion;
                    worksheet.Cells[row, 2].Value = item.ClaseCampania.Descripcion;
                    worksheet.Cells[row, 3].Value = item.FechaCreacion.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 4].Value = item.FechaPlanificado?.ToString("dd/MM/yyyy hh:mm");
                    worksheet.Cells[row, 5].Value = item.EstadoCampania.Descripcion;
                    worksheet.Cells[row, 6].Value = item.TemplateName;
                    worksheet.Cells[row, 7].Value = item.Asunto;
                    worksheet.Cells[row, 8].Value = item.CantidadEmails;
                    worksheet.Cells[row, 9].Value = item.CantidadSuministros;
                    row++;
                }
                package.Save();
            }

            stream.Position = 0;
            response.FileStream = stream.ToArray();
            return ServiceResponseFactory.CreateOkResponse(response);

        }

        public async Task<ServiceResponse<ExportFileResponse>> DownloadExcelDemoAsync()
        {
            var response = new ExportFileResponse { Title = "Template_archivo_csv.csv" };
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                package.Workbook.Properties.Title = "Template_archivo_csv";
                var worksheet = package.Workbook.Worksheets.Add("Hoja1");
                worksheet.Name = "Hoja 1";
                var row = 1;
                worksheet.Cells[row, 1].Value = "Contacto";
                worksheet.Cells[row, 2].Value = "Variable1";
                worksheet.Cells[row, 3].Value = "Variable2";
                worksheet.Cells[row, 4].Value = "Variable3";
                worksheet.Cells[row, 5].Value = "Variable4";
                package.Save();
            }

            stream.Position = 0;
            response.FileStream = stream.ToArray();
            return ServiceResponseFactory.CreateOkResponse(response);
        }

        private static bool? NumericToBoolean(int number)
        {
            switch (number)
            {
                case -1: return null;
                case 0: return false;
                case 1: return true;
                default: return null;
            }
        }

        //public async Task<CampaniaDto> GetCampaniaHtmlAsync(long id)
        //{
        //    var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == id, c => c.CampaniasHtmlVariables);
        //    var campaniaDto = _mapper.Map<CampaniaDto>(campania);
        //    return ServiceResponseFactory.CreateOkResponse(campaniaDto);
        //}

        public async Task<CampaniaDto> GetCampaniaHtmlAsync(long id)
        {
            var campania = await _unitOfWork.CampaniaRepository().FindByAsync(f => f.IdCampania == id,
                c => c.TipoCampania,                
                c => c.ParametrosCampanias,                
                c => c.CampaniasHtmlVariables);
            var campaniaDto = _mapper.Map<CampaniaDto>(campania);
            if (campaniaDto.TipoSeleccion == 1 || campaniaDto.TipoSeleccion == 4)
            {
                await GetParametrosAsync(campaniaDto);
            }
           
            return campaniaDto;
        }

    }
}
