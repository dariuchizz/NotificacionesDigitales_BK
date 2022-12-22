using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common.Model.Directory;
using Common.Model.Dto;
using Common.Model.NotificacionesDigitales;

namespace Common
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Suministro, SuministroDto>();
            CreateMap<SuministroDto, Suministro>();
            CreateMap<Factura, FacturaDto>();
            CreateMap<FacturaDto, Factura>();
            CreateMap<Email, EmailDto>();
            CreateMap<EmailDto, Email>();
            CreateMap<TipoComunicacion, TipoComunicacionDto>();
            CreateMap<TipoComunicacionDto, TipoComunicacion>();
            CreateMap<Comunicacion, ComunicacionDto>();
            CreateMap<ComunicacionDto, Comunicacion>();
            CreateMap<EventoEmail, EventoEmailDto>();
            CreateMap<EventoEmailDto, EventoEmail>();
            CreateMap<EventoSMS, EventoSMSDto>();
            CreateMap<EventoSMSDto, EventoSMS>();
            CreateMap<ProcesoEvento, ProcesoEventoDto>();
            CreateMap<ProcesoEventoDto, ProcesoEvento>();
            CreateMap<CambioContacto, CambioContactoDto>();
            CreateMap<CambioContactoDto, CambioContacto>();
            CreateMap<ReenvioEmailFallado, ReenvioEmailFalladoDto>();
            CreateMap<ReenvioEmailFalladoDto, ReenvioEmailFallado>();
            CreateMap<ConfiguracionEmail, ConfiguracionEmailDto>();
            CreateMap<ConfiguracionEmailDto, ConfiguracionEmail>();
            CreateMap<AvisosDeudasDetalle, AvisoDeudaDetalleDto>();
            CreateMap<AvisoDeudaDetalleDto, AvisosDeudasDetalle>();
            CreateMap<AvisosDeuda, AvisoDeudaDto>();
            CreateMap<AvisoDeudaDto, AvisosDeuda>();
            CreateMap<ListaGris, ListaGrisDto>();
            CreateMap<ListaGrisDto, ListaGris>();
            CreateMap<MotivoBajaListaGris, MotivoBajaListaGrisDto>();
            CreateMap<MotivoBajaListaGrisDto, MotivoBajaListaGris>();
            CreateMap<ConfiguracionEmail, EventoAccionesFormularioDto>()
                .ForMember(d => d.Activo, o => o.MapFrom(src => src.Activo))
                .ForMember(d => d.Codigo, o => o.MapFrom(src => src.Code))
                .ForMember(d => d.CodigoRechazo, o => o.MapFrom(src => src.BounceCode))
                .ForMember(d => d.Razon, o => o.MapFrom(src => src.Reason))
                .ForMember(d => d.Severidad, o => o.MapFrom(src => src.Severity))
                .ForMember(d => d.IdEventoResultante, o => o.MapFrom(src => src.IdEventoResultanteEmail));
            CreateMap<EventosResultantesEmail, ComboLongDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.IdEventoResultanteEmail))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => src.Resultante));
            CreateMap<Campania, CampaniaDto>()
                .ForMember(d => d.TemplateName, o => o.MapFrom(src => src.Template))
                .ForMember(d => d.TipoCampania, o => o.MapFrom(src => src.TipoCampania))
                .ForMember(d => d.EstadoCampania, o => o.MapFrom(src => src.EstadoCampania))
                .ForMember(d => d.CsvCampanias, o => o.MapFrom(src => src.CsvCampanias))
                .ForMember(d => d.ParametrosCampania, o => o.MapFrom(src => src.ParametrosCampanias.FirstOrDefault()))
                .ForMember(d => d.CampaniasHtmlVariableDto, o => o.MapFrom(src => src.CampaniasHtmlVariables.FirstOrDefault()))
                .ForMember(d => d.ClaseCampaniaSeleccion, o => o.MapFrom(src => src.IdClaseCampania))
                .ForMember(d => d.CanalCampaniaSeleccion, o => o.MapFrom(src => src.IdCanalCampania))
                .ForMember(d => d.TipoSeleccion, o => o.MapFrom(src => src.IdTipoCampania));
            CreateMap<TipoCampania, TipoCampaniaDto>();
            CreateMap<TipoCampaniaDto, TipoCampania>();
            CreateMap<EstadoCampania, EstadoCampaniaDto>();
            CreateMap<EstadoCampaniaDto, EstadoCampania>();
            CreateMap<CampaniasHtmlVariable, CampaniasHtmlVariableDto>();
            CreateMap<CampaniasHtmlVariableDto, CampaniasHtmlVariable>();
            CreateMap<EnvioSmsGenerico, EnvioSmsGenericoDto>();
            CreateMap<EnvioSmsGenericoDto, EnvioSmsGenerico>();
            CreateMap<ProcesoNegocio, ProcesoNegocioDto>();
            CreateMap<ProcesoNegocioDto, ProcesoNegocio>();
            CreateMap<Sistema, SistemaDto>();
            CreateMap<SistemaDto, Sistema>();
            CreateMap<ParametrosCampania, ParametrosCampaniasDto>()
                .ForMember(d => d.Estados, o => o.MapFrom(src => SplitStringByComma(src.Estados)))
                .ForMember(d => d.CategoriasDetalle, o => o.MapFrom(src => SplitStringByComma(src.Categorias)))
                .ForMember(d => d.Localidades, o => o.MapFrom(src => SplitStringByComma(src.Localidades)))
                .ForMember(d => d.EnteOficial, o => o.MapFrom(src => BooleanToNumeric(src.EnteOficial)))
                .ForMember(d => d.GranCliente, o => o.MapFrom(src => BooleanToNumeric(src.GranCliente)))
                .ForMember(d => d.TieneDebitoAutomatico, o => o.MapFrom(src => BooleanToNumeric(src.TieneDebitoAutomatico)))
                .ForMember(d => d.TieneNotificacionDigital, o => o.MapFrom(src => BooleanToNumeric(src.TieneNotificacionDigital)));

            CreateMap<ParametrosCampaniasDto, ParametrosCampania>()
                .ForMember(d => d.EnteOficial, o => o.MapFrom(src => NumericToBoolean(src.EnteOficial)))
                .ForMember(d => d.GranCliente, o => o.MapFrom(src => NumericToBoolean(src.GranCliente)))
                .ForMember(d => d.TieneDebitoAutomatico, o => o.MapFrom(src => NumericToBoolean(src.TieneDebitoAutomatico)))
                .ForMember(d => d.TieneNotificacionDigital, o => o.MapFrom(src => NumericToBoolean(src.TieneNotificacionDigital)));
            CreateMap<CsvCampaniaDto, CsvCampania>();
            CreateMap<CsvCampania, CsvCampaniaDto>()
                .ForMember(d => d.Apellido, o => o.MapFrom(src => src.Apellido))
                .ForMember(d => d.Domicilio, o => o.MapFrom(src => src.Domicilio))
                .ForMember(d => d.Nombre, o => o.MapFrom(src => src.Nombre))
                .ForMember(d => d.NombreApellido, o => o.MapFrom(src => src.NombreApellido))
                ;
            CreateMap<CampaniaDto, Campania>()
                .ForMember(d => d.Template, o => o.MapFrom(src => src.TemplateName))
                .ForMember(d => d.TipoCampania, o => o.MapFrom(src => src.TipoCampania))
                .ForMember(d => d.EstadoCampania, o => o.MapFrom(src => src.EstadoCampania))
                .ForMember(d => d.CsvCampanias, o => o.MapFrom(src => src.CsvCampanias))
                .ForMember(d => d.IdClaseCampania, o => o.MapFrom(src => src.ClaseCampaniaSeleccion))                
                .ForMember(d => d.ParametrosCampanias, o => o.MapFrom(src => src.ParametrosCampania != null ? new List<ParametrosCampaniasDto> { src.ParametrosCampania } : new List<ParametrosCampaniasDto>()));
            
            CreateMap<Canal, ComboLongDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.IdCanal))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => src.Descripcion));

            CreateMap<EstadoCampania, ComboLongDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.IdEstadoCampania))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => src.Descripcion));
            CreateMap<EstadosSuministro, ComboStringDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.Codigo.Trim()))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => $"{src.Codigo} - {src.Descripcion}"));
            CreateMap<CategoriasSuministro, ComboStringDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.Codigo.Trim()))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => $"{src.Codigo} - {src.Descripcion}"));
            //MAPPER PARA DUPLICIDAD DE LA CAMPANIA
            CreateMap<Campania, Campania>()
                .ForMember(d => d.TipoCampania, o => o.Ignore())
                .ForMember(d => d.IdTipoCampania, o => o.MapFrom(src=> src.IdTipoCampania))
                .ForMember(d => d.IdCampania, o => o.Ignore())
                .ForMember(d => d.EstadoCampania, o => o.Ignore())
                .ForMember(d => d.IdEstadoCampania, o => o.Ignore())
                .ForMember(d => d.ParametrosCampanias, o => o.MapFrom(m => m.ParametrosCampanias))
                .ForMember(d => d.CampaniasHtmlVariables, o => o.MapFrom(m => m.CampaniasHtmlVariables))
                .ForMember(d => d.CsvCampanias, o => o.MapFrom(m => m.CsvCampanias));
            CreateMap<CsvCampania, CsvCampania>()
                .ForMember(d => d.IdCsvCampania, o => o.Ignore())
                .ForMember(d => d.IdCampania, o => o.Ignore());
            CreateMap<ParametrosCampania, ParametrosCampania>()
                .ForMember(d => d.IdParametro, o => o.Ignore())
                .ForMember(d => d.IdCampania, o => o.Ignore());
            //
            CreateMap<BusinessUnit, ComboStringDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => src.Code.Trim()))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => src.Name));
            CreateMap<Agcpostlpf, ComboStringDto>()
                .ForMember(d => d.Id, o => o.MapFrom(src => $"{src.Agcpcodigo}-{src.Agcpdigito}"))
                .ForMember(d => d.Descripcion, o => o.MapFrom(src => src.Agcplocali.Trim()));
            CreateMap<CampaniasHtmlVariable, CampaniasHtmlVariable>()
                .ForMember(d => d.IdCampania, o => o.Ignore())
                .ForMember(d => d.IdCampaniaHtmlVariable, o => o.Ignore());

        }

        private static List<string> SplitStringByComma(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<string>();
            }

            return value.Split(',').ToList();
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

        private static int BooleanToNumeric(bool? boolean)
        {
            switch (boolean)
            {
                case null: return -1;
                case false: return 0;
                case true: return 1;
                default: return -1;
            }
        }
    }
}
