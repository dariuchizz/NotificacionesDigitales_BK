using System;
using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class AvisoDeudaDto
    {
        public long IdAvisoDeuda { get; set; }
        public long IdSuministro { get; set; }
        public int Emisor { get; set; }
        public string Numero { get; set; }
        public short Empresa { get; set; }
        public decimal Total { get; set; }
        public decimal Saldo { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public bool? Pagado { get; set; }
        public bool? Anulado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public IEnumerable<AvisoDeudaDetalleDto> AvisosDeudasDetalles { get; set; }
    }
}
