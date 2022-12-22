using System;
using System.Collections.Generic;

namespace Common.Model.Dto
{
    public class SuministroDto
    {
        public SuministroDto()
        {
            this.Emails = new List<EmailDto>();
            this.Facturas = new List<FacturaDto>();
        }
        public long IdSuministro { get; set; }
        public long IdEmprea { get; set; }
        public long IdExterno { get; set; }
        public long CuentaUnificada { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto { get; set; }
        public bool TieneDebitoAutomatico { get; set; }
        public string Domicilio { get; set; }
        public int CodigoPostal { get; set; }
        public int SubCodigo { get; set; }
        public decimal? GrupoFacturacion { get; set; }
        public decimal? RutaFacturacion { get; set; }
        public decimal? OrdenFacturacion { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public bool? GranCliente { get; set; }
        public bool? EnteOficial { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
        public IEnumerable<EmailDto> Emails { get; set; }

        public IEnumerable<FacturaDto> Facturas { get; set; }
    }
}
