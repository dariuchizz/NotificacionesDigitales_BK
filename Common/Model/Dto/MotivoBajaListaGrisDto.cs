using System;

namespace Common.Model.NotificacionesDigitales
{
    public class MotivoBajaListaGrisDto
    {
        public long IdMotivoBajaListaGris { get; set; }

        public string Descripcion { get; set; }

        public bool RequiereObservacion { get; set; }

        public int Autor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? AutorModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }
}
