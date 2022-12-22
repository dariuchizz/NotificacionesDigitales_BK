using Newtonsoft.Json;
using Common.Model.Enum;
using System;

namespace Common.Model.Dto
{
    public class DynamicDto
    {
        public long IdContacto { get; set; }
        public long IdRelacionado { get; set; }
        public long IdExterno { get; set; }
        public string numeroFactura { get; set; }
        public string Email { get; set; }
        public long idComunicacion { get; set; }
        public Canal IdCanal { get; set; }
        public string Celular { get; set; }
        public string Message { get; set; }
        [JsonProperty("cuentaUnificada")]
        public string CuentaUnificada { get; set; }
        public string cuentaUnificadaFormat { get; set; }
        public string JsonData { get; set; }
        public string fechaVencimiento { get; set; }
        public string importe { get; set; }
        public string nombreApellido { get; set; }
        public string linkHuemul { get; set; }

    }
}