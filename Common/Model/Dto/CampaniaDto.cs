using System;
using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class CampaniaDto
    {
        public long IdCampania { get; set; }
        
        public string Descripcion { get; set; }
        
        public int TopeLectura { get; set; }
        
        public string TemplateName { get; set; }
        
        public string TemplateHtml { get; set; }
        
        public string Asunto { get; set; }
        
        public string Tag { get; set; }
        
        public int Autor { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public int? AutorModificacion { get; set; }
        
        public DateTime? FechaModificacion { get; set; }
        
        public bool Activo { get; set; }
        
        public string IdScheduleGenerar { get; set; }
        
        public string IdScheduleObtener { get; set; }
        
        public long CantidadEmails { get; set; }
        
        public long CantidadSuministros { get; set; }
        
        public TipoCampaniaDto TipoCampania { get; set; }
        
        public IEnumerable<CsvCampaniaDto> CsvCampanias { get; set; }

        public string PathFullCsv { get; set; }

        public ParametrosCampaniasDto ParametrosCampania { get; set; }
        
        public EstadoCampaniaDto EstadoCampania { get; set; }
        
        public long IdEstadoCampania { get; set; }
        
        public long ActualStep { get; set; }
        
        public DateTime? FechaPlanificado { get; set; }
        
        public long TipoSeleccion { get; set; }
        public int ClaseCampaniaSeleccion { get; set; }

        public long CantidadRegistrosCsv { get; set; }
        
        public CampaniasHtmlVariableDto CampaniasHtmlVariableDto { get; set; }
        
        public bool HasChangeStep2 { get; set; }
        public ClaseCampaniaDto ClaseCampania { get; set; }
        public int IdClaseCampania { get; set; }

        public int IdCanalCampania { get; set; }
        public int CanalCampaniaSeleccion { get; set; }

        public string TextoSMS { get; set; }

    }
}
