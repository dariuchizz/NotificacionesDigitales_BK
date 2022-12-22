namespace Common.Model.Dto
{
    public class EnvioSMSRequest
    {
        public string NroCelular { get; set; }

        public string TextoMensaje { get; set; }

        public long IdSistema { get; set; }

        public long IdProcesoNegocio { get; set; }

        public string Datos { get; set; }
    }
}
