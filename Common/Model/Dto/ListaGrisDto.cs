using System;

namespace Common.Model.NotificacionesDigitales
{

    public class ListaGrisDto
    {
        public long IdListaGris { get; set; }

        public long IdComunicacion { get; set; }

        public long IdMotivo { get; set; }

        public string ObservacionCliente { get; set; }

        public string Origen { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }
}
