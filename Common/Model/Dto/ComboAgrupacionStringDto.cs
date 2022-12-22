using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class ComboAgrupacionStringDto
    {
        public string Agrupacion { get; set; }
        public IEnumerable<ComboStringDto> Detalle { get; set; }
    }
}
