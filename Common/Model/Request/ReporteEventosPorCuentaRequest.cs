namespace Common.Model.Request
{
    public class ReporteEventosPorCuentaRequest: GridRequest
    {
        public string CuentaUnificada { get; set; }
        public long IdSuministro { get; set; }
        public override string ToString()
        {
            return $"CuentaUnificada=${CuentaUnificada}" +
                   $"&PageIndex={PageIndex}&PageSize={PageSize}&Active={Active}&Direction={Direction}";
        }
    }
}
