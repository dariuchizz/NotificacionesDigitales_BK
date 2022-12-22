namespace Common.Model.Dto
{
    public class EventosAccionesDto
    {
        public long Id { get; set; }
        public string Razon { get; set; }
        public int Codigo { get; set; }
        public string CodigoRechazo { get; set; }
        public string Severidad { get; set; }
        public string ResultanteRechazo { get; set; }
        public bool Activo { get; set; }
    }
}
