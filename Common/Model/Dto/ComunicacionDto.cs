using System;
using Common.Model.NotificacionesDigitales;
using Newtonsoft.Json;

namespace Common.Model.Dto
{
    public class ComunicacionDto
    {
        [JsonProperty("idComunicacion")]
        public long IdComunicacion { get; set; }

        [JsonProperty("idTipoComunicacion")]
        public long IdTipoComunicacion { get; set; }

        [JsonProperty("idCanal")]
        public long IdCanal { get; set; }

        [JsonProperty("idSuministro")]
        public long IdSuministro { get; set; }

        [JsonProperty("idRelacionado")]
        public long IdRelacionado { get; set; }

        [JsonProperty("idContacto")]
        public long IdContacto { get; set; }

        [JsonProperty("procesado")]
        public int Procesado { get; set; }

        [JsonProperty("estadoEnviado")]
        public int Enviado { get; set; }

        [JsonProperty("fechaProceso")]
        public DateTime? FechaProceso { get; set; }

        [JsonProperty("idExterno")]
        public string IdExterno { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("idExternoPadre ")]
        public string IdExternoPadre { get; set; }

        [JsonProperty("guid")]
        public Guid? GUID { get; set; }

        [JsonProperty("autor")]
        public int Autor { get; set; }

        [JsonProperty("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonProperty("autorModificacion")]
        public int? AutorModificacion { get; set; }

        [JsonProperty("fechaModificacion")]
        public DateTime? FechaModificacion { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("IdEnvio")]
        public long IdEnvio { get; set; }

        public Envio Envio { get; set; }
    }
}
