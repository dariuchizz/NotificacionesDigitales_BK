using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Suministros")]
    public class Suministro
    {
        public Suministro()
        {
            this.Emails = new List<Email>();
            this.Facturas = new List<Factura>();
        }

        [Column("IdSuministro", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdSuministro { get; set; }

        [Column("IdEmpresa", TypeName = "bigint")]
        [Required]
        public long IdEmprea { get; set; }

        [Column("IdExterno", TypeName = "bigint")]
        [Required]
        public long IdExterno { get; set; }

        [Column("CuentaUnificada", TypeName = "bigint")]
        [Required]
        public long CuentaUnificada { get; set; }

        [Column("Nombre", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }

        [Column("Apellido", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Apellido { get; set; }

        [Column("NombreCompleto", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string NombreCompleto { get; set; }

        [Column("TieneDebitoAutomatico", TypeName = "bit")]
        [Required]
        public bool TieneDebitoAutomatico { get; set; }

        [Column("Domicilio", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        [Required]
        public string Domicilio { get; set; }

        [Column("CodigoPostal", TypeName = "int")]
        [Required]
        public int CodigoPostal { get; set; }

        [Column("SubCodigo", TypeName = "smallint")]
        public int SubCodigo { get; set; }

        [Column("GrupoFacturacion", TypeName = "numeric")]
        public decimal? GrupoFacturacion { get; set; }

        [Column("RutaFacturacion", TypeName = "numeric")]
        public decimal? RutaFacturacion { get; set; }

        [Column("OrdenFacturacion", TypeName = "decimal")]
        public decimal? OrdenFacturacion { get; set; }

        [Column("Categoria", TypeName = "varchar(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Categoria { get; set; }

        [Column("Estado", TypeName = "varchar(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Estado { get; set; }

        [Column("GranCliente", TypeName = "bit")]
        public bool? GranCliente { get; set; }

        [Column("EnteOficial", TypeName = "bit")]
        public bool? EnteOficial { get; set; }

        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }

        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }

        [Column("Activo", TypeName = "bit")]
        [Required]
        public bool Activo { get; set; }

        public IEnumerable<Email> Emails { get; set; }

        public IEnumerable<Factura> Facturas { get; set; }
    }

}
