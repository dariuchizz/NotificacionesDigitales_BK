using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.Directory
{
    [Table("AGCPOSTLPF")]
    public class Agcpostlpf
    {
        [Column("AGCPCODIGO", TypeName = "char(4)", Order = 1)]
        [Key]
        [MaxLength(4)]
        [StringLength(4)]
        [Required]
        public string Agcpcodigo { get; set; }

        [Column("AGCPDIGITO", TypeName = "char(1)", Order = 2)]
        [Key]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcpdigito { get; set; }
        [Column("AGCPLOCALI", TypeName = "varchar(25)")]
        [MaxLength(25)]
        [StringLength(25)]
        [Required]
        public string Agcplocali { get; set; }
        [Column("AGCPCODMUN", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Agcpcodmun { get; set; }
        [Column("AGCPMUNICP", TypeName = "varchar(25)")]
        [MaxLength(25)]
        [StringLength(25)]
        [Required]
        public string Agcpmunicp { get; set; }
        [Column("AGCPPROVIN", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Agcpprovin { get; set; }
        [Column("AGCPDESCRI", TypeName = "varchar(25)")]
        [MaxLength(25)]
        [StringLength(25)]
        [Required]
        public string Agcpdescri { get; set; }
        [Column("AGCPZONTAR", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Agcpzontar { get; set; }
        [Column("AGCPULTCTA", TypeName = "int")]
        [Required]
        public int Agcpultcta { get; set; }
        [Column("AGCPCODLOC", TypeName = "char(5)")]
        [MaxLength(5)]
        [StringLength(5)]
        [Required]
        public string Agcpcodloc { get; set; }
        [Column("AGCPCODUNI", TypeName = "char(5)")]
        [MaxLength(5)]
        [StringLength(5)]
        [Required]
        public string Agcpcoduni { get; set; }
        [Column("AGCPDOMICI", TypeName = "varchar(20)")]
        [MaxLength(20)]
        [StringLength(20)]
        [Required]
        public string Agcpdomici { get; set; }
        [Column("AGCPNUMERO", TypeName = "char(4)")]
        [MaxLength(4)]
        [StringLength(4)]
        [Required]
        public string Agcpnumero { get; set; }
        [Column("AGCPPISO", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        [Required]
        public string Agcppiso { get; set; }
        [Column("AGCPTELEFO", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        [Required]
        public string Agcptelefo { get; set; }
        [Column("AGCPNROFAC", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        [Required]
        public string Agcpnrofac { get; set; }
        [Column("AGCPNROCRE", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        [Required]
        public string Agcpnrocre { get; set; }
        [Column("AGCPNRODEB", TypeName = "varchar(10)")]
        [MaxLength(10)]
        [StringLength(10)]
        [Required]
        public string Agcpnrodeb { get; set; }
        [Column("AGCEXEINAC", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcexeinac { get; set; }
        [Column("AGCEXEIPRO", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcexeipro { get; set; }
        [Column("AGCEXEIMUN", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcexeimun { get; set; }
        [Column("AGCPDATOS1", TypeName = "varchar(79)")]
        [MaxLength(79)]
        [StringLength(79)]
        [Required]
        public string Agcpdatos1 { get; set; }
        [Column("AGCPDATOS2", TypeName = "varchar(79)")]
        [MaxLength(79)]
        [StringLength(79)]
        [Required]
        public string Agcpdatos2 { get; set; }
        [Column("AGCPDATOS3", TypeName = "varchar(79)")]
        [MaxLength(79)]
        [StringLength(79)]
        [Required]
        public string Agcpdatos3 { get; set; }
        [Column("AGCPDATOS4", TypeName = "varchar(79)")]
        [MaxLength(79)]
        [StringLength(79)]
        [Required]
        public string Agcpdatos4 { get; set; }
        [Column("AGCPDATOS5", TypeName = "varchar(79)")]
        [MaxLength(79)]
        [StringLength(79)]
        [Required]
        public string Agcpdatos5 { get; set; }
        [Column("AGCPFILLER", TypeName = "char(12)")]
        [MaxLength(12)]
        [StringLength(12)]
        [Required]
        public string Agcpfiller { get; set; }
        [Column("AGCPROINAC", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcproinac { get; set; }
        [Column("AGCPROIPRO", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcproipro { get; set; }
        [Column("AGCPROIMUN", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        [Required]
        public string Agcproimun { get; set; }
        [Column("AGCPEMPRES", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        public string Agcpempres { get; set; }
        [Column("AGCPUNINEG", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        public string Agcpunineg { get; set; }
        [Column("AGCPCENOPE", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        public string Agcpcenope { get; set; }
        [Column("AGCPCENGES", TypeName = "char(3)")]
        [MaxLength(3)]
        [StringLength(3)]
        public string Agcpcenges { get; set; }
        [Column("AGCPUNIORI", TypeName = "int")]
        public int? Agcpuniori { get; set; }
        [Column("AGCPMARCAX", TypeName = "char(1)")]
        [MaxLength(1)]
        [StringLength(1)]
        public string Agcpmarcax { get; set; }
    }

}
