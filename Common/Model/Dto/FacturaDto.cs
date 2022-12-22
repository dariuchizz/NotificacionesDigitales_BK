using System;

namespace Common.Model.Dto
{
    public class FacturaDto
    {
        public long IdFactura { get; set; }
        public long IdSuministro { get; set; }
        public string NroFactura { get; set; }
        public decimal Total { get; set; }
        public decimal Saldo { get; set; }
        public bool Pagada { get; set; }
        public bool Anulada { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaPago { get; set; }
        public int Autor { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? AutorModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }
}
