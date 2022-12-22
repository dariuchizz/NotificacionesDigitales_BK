namespace Common.Model.Dto
{
    public class TestMailgunDto
    {
        public int TipoEnvio { get; set; }
        public string Email { get; set; }
        public string DomicilioJson { get; set; }
        public string NombreJson { get; set; }
        public string ApellidoJson { get; set; }
        public string NombreCompletoJson { get; set; }
        public string DomicilioValor { get; set; }
        public string NombreValor { get; set; }
        public string ApellidoValor { get; set; }
        public string NombreCompletoValor { get; set; }
        public long TipoComunicacion { get; set; }

        public bool IsByCsv { get; set; }

        public bool HasVariables { get; set; }
        //"{\"email\":\"rhuberman@xsidesolutions.com\",\"tipoComunicacion\":\"7\"}"
    }
}
