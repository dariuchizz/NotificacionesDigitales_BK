using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model.Dto
{
    public class ComboGrupoCategoriasDto
    {
        public bool GranCliente { get; set; }
        public string Descripcion { get; set; }
        public IEnumerable<ComboAgrupacionStringDto> DetalleGrupo { get; set; }
    }
}
