namespace Common.Model.Dto
{
    public class TestSMSDto
    {
        public int TipoEnvio { get; set; }
        
        public string Celular { get; set; }

        public string NombreValor { get; set; }

        public long TipoComunicacion { get; set; }

        public bool IsByCsv { get; set; }

        public bool HasVariables { get; set; }
    }
}
