using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Model.NotificacionesDigitales
{
    [Table("Comunicaciones")]
    public class Comunicacion
    {
        public Comunicacion()
        {
            this.EventosComunicaciones = new HashSet<EventosComunicaciones>();
            this.ListasGrises = new HashSet<ListaGris>();
        }

        [Column("IdComunicacion", TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long IdComunicacion { get; set; }
        
        [Column("IdEnvio", TypeName = "bigint")]
        public long? IdEnvio { get; set; }

        [Column("IdCanal", TypeName = "bigint")]
        [Required]
        public long IdCanal { get; set; }

        [Column("IdSuministro", TypeName = "bigint")]
        public long? IdSuministro { get; set; }

        [Column("IdRelacionado", TypeName = "bigint")]
        [Required]
        public long IdRelacionado { get; set; }

        [Column("IdContacto", TypeName = "bigint")]
        [Required]
        public long IdContacto { get; set; }

        [Column("Procesado", TypeName = "bit")]
        [Required]
        public Boolean Procesado { get; set; }

        [Column("Enviado", TypeName = "int")]
        [Required]
        public int Enviado { get; set; }

        [Column("FechaProceso", TypeName = "datetime")]
        public DateTime? FechaProceso { get; set; }

        [Column("IdExterno", TypeName = "varchar(100)")]
        [MaxLength]
        public string IdExterno { get; set; }

        [Column("Message", TypeName = "varchar(256)")]
        [MaxLength]
        public string Message { get; set; }

        [Column("IdExternoPadre", TypeName = "varchar(100)")]
        [MaxLength]
        public string IdExternoPadre { get; set; }

        [Column("GUID", TypeName = "uniqueidentifier")]
        public Guid? GUID { get; set; }

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

        [ForeignKey("IdCanal")]
        public Canal Canale { get; set; }

        public ICollection<EventosComunicaciones> EventosComunicaciones { get; set; }

        public ICollection<ListaGris> ListasGrises { get; set; }

        [ForeignKey("IdEnvio")]
        public Envio Envio { get; set; }

    }
}