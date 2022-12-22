using System;

namespace Common.Model.Dto
{
    public class ReenvioEmailFalladoDto
    {
        public long IdReenvioEmailFallado { get; set; }

        public long? IdComunicacion { get; set; }

        public long? IdEventoEmail { get; set; }

        public long? IdConfiguracionEmail { get; set; }

        public bool? Procesado { get; set; }
        
        public DateTime FechaProcesado { get; set; }

        public DateTime FechaCreacion { get; set; }

    }
}
