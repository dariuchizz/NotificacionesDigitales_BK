using System;

namespace Common.Model.Dto
{
    public class TipoComunicacionDto
    {
        public long IdTipoComunicacion { get; set; }

        public bool Aviso { get; set; }

        public string Descripcion { get; set; }
        
        public string Template { get; set; }
        public string TemplateHtml { get; set; }

        public string Asunto { get; set; }
        
        public string Tag { get; set; }
        
        public string StoreGenerarComunicaciones { get; set; }
        
        public string StoreObtenerLote { get; set; }
        
        public int TopeLectura { get; set; }
        
        public int Autor { get; set; }
        
        public DateTime FechaCreacion { get; set; }
        
        public int? AutorModificacion { get; set; }
        
        public DateTime? FechaModificacion { get; set; }
        
        public bool Activo { get; set; }
    }
}
