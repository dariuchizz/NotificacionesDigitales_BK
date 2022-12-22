using Newtonsoft.Json;
using System;
using Azure.Storage.Queues.Models;

namespace Common.Model.Dto
{
    public class NotificacionHuemulDto
    {
        [JsonProperty("idHuemulComunicacion")]
        public long IdHuemulComunicacion { get; set; }

        [JsonProperty("cuentaUnificada")]
        public long CuentaUnificada { get; set; }

        [JsonProperty("tipoComprobante")]
        public string TipoComprobante { get; set; }

        [JsonProperty("nroComprobante")]
        public string NroComprobante { get; set; }

        [JsonProperty("canal")]
        public string Canal { get; set; }

        [JsonProperty("linkHuemul")]
        public string LinkHuemul { get; set; }

        [JsonProperty("actualizadosDesdeAG")]
        public bool ActualizadosDesdeAG { get; set; }

        [JsonProperty("fechaProcesado")]
        public DateTime? FechaProcesado { get; set; }

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

        public QueueMessage messageHuemul { get; set; }
    }
}