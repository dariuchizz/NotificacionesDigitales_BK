using System;

namespace Common.Model.Dto
{
    public class SistemaDto
    {
        public long IdSistema { get; set; }

        public string Descripcion { get; set; }

        public int Autor { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int? AutorModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }
    }
}
