using System;

namespace Common.Model.Request
{
    public class CobrabilidadRequest: GridRequest
    {
        public string IdEnvios { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public long TipoComunicacion { get; set; }
        public override string ToString()
        {
            return $"IdEnvios={IdEnvios}&DateFrom={DateFrom:yyyy/MM/dd}&DateTo={DateTo:yyyy/MM/dd}&TipoComunicacion={TipoComunicacion}" +
                   $"&PageIndex={PageIndex}&PageSize={PageSize}&Active={Active}&Direction={Direction}";
        }

    }
}
