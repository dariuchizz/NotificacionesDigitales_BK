using System;

namespace Common.Model.Dto
{
    public class EventoAccionesFormularioDto: EventosAccionesDto
    {
        public long IdEventoResultante { get; set; }
        public int Autor { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int AutorModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
