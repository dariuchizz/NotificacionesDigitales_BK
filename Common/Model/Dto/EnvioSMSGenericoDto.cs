using System;

namespace Common.Model.Dto
{
    public class EnvioSmsGenericoDto
    {
        public long IdEnvioSMSGenerico { get; set; }

        public string NroCelular { get; set; }

        public string TextoMensaje { get; set; }

        public long IdSistema { get; set; }

        public long IdProcesoNegocio { get; set; }

        public string Datos { get; set; }

        public DateTime? FechaProceso { get; set; }

        public int Autor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? AutorModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }
}
