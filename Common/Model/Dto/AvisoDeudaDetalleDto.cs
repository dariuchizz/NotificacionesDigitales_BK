namespace Common.Model.Dto
{
    public class AvisoDeudaDetalleDto
    {
        public long IdAvisoDeuda { get; set; }
        public long IdFactura { get; set; }
        public FacturaDto Factura { get; set; }

    }
}
