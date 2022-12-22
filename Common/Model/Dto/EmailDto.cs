using System;

namespace Common.Model.Dto
{
    public class EmailDto
    {
        public long IdEmail { get; set; }
        
        public long? IdSuministro { get; set; }
        
        public long? IdExterno { get; set; }
        
        public string DEmail { get; set; }
        
        public bool? TieneNotificacionDigital { get; set; }
        
        public long? IdMotivoBaja { get; set; }
        
        public string EsTitular { get; set; }
        
        public int Autor { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public int? AutorModificacion { get; set; }
        
        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }
}
