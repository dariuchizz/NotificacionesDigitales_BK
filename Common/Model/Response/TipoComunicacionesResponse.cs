using System.Collections.Generic;

namespace Common.Model.Response
{
    public class TipoComunicacionesResponse
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public IEnumerable<long> Envios { get; set; }
    }
}
