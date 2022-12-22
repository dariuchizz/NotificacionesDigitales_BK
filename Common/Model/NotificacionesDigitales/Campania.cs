using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Campanias")]
    public class Campania
    {
        public Campania()
        {
            this.CsvCampanias = new HashSet<CsvCampania>();
            this.ParametrosCampanias = new HashSet<ParametrosCampania>();
            this.CampaniasHtmlVariables = new HashSet<CampaniasHtmlVariable>();
        }

        [Key]
        [Column("IdCampania", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdCampania { get; set; }

        [Column("Descripcion", TypeName = "varchar(50)")]
        [MaxLength(50)]
        [StringLength(50)]
        public string Descripcion { get; set; }
        [Column("TopeLectura", TypeName = "int")]
        [Required]
        public int TopeLectura { get; set; }
        [Column("IdEstadoCampania", TypeName = "bigint")]
        [Required]
        public long IdEstadoCampania { get; set; }
        [Column("IdTipoCampania", TypeName = "bigint")]
        [Required]
        public long IdTipoCampania { get; set; }
        [Column("Template", TypeName = "varchar(100)")]
        [MaxLength(100)]
        [StringLength(100)]
        [Required]
        public string Template { get; set; }
        [Column("Asunto", TypeName = "varchar(255)")]
        [MaxLength(255)]
        [StringLength(255)]
        public string Asunto { get; set; }
        [Column("Tag", TypeName = "varchar(30)")]
        [MaxLength(30)]
        [StringLength(30)]
        public string Tag { get; set; }
        [Column("Autor", TypeName = "int")]
        [Required]
        public int Autor { get; set; }
        [Column("FechaCreacion", TypeName = "datetime")]
        [Required]
        public DateTime FechaCreacion { get; set; }
        [Column("AutorModificacion", TypeName = "int")]
        public int? AutorModificacion { get; set; }
        [Column("FechaModificacion", TypeName = "datetime")]
        public DateTime? FechaModificacion { get; set; }
        [Column("Activo", TypeName = "bit")]
        [Required]
        public bool Activo { get; set; }
        [Column("IdScheduleGenerar", TypeName = "varchar(max)")]
        [MaxLength]
        public string IdScheduleGenerar { get; set; }
        [Column("IdScheduleObtener", TypeName = "varchar(max)")]
        [MaxLength]
        public string IdScheduleObtener { get; set; }
        [Column("CantidadEmails", TypeName = "bigint")]
        public long CantidadEmails { get; set; }
        [Column("CantidadSuministros", TypeName = "bigint")]
        public long CantidadSuministros { get; set; }
        [Column("FechaPlanificado", TypeName = "datetime")]
        public DateTime? FechaPlanificado { get; set; }

        [Column("IdClaseCampania", TypeName = "bigint")]
        [Required]
        public int IdClaseCampania { get; set; }

        public Envio Envio { get; set; }
        public EstadoCampania EstadoCampania { get; set; }
        public TipoCampania TipoCampania { get; set; }
        public ICollection<CsvCampania> CsvCampanias { get; set; }
        public ICollection<ParametrosCampania> ParametrosCampanias { get; set; }
        public ICollection<CampaniasHtmlVariable> CampaniasHtmlVariables { get; set; }
        public ClaseCampania ClaseCampania { get; set; }

        [Column("IdCanalCampania", TypeName = "bigint")]
        [Required]
        public int IdCanalCampania { get; set; }
        public string TextoSMS { get; set; }
    }

}
