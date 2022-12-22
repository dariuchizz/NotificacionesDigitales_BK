namespace Common.Model.Request
{
    public class NotificacionRequest: GridRequest
    {
        public string cuentaUnificada { get; set; }
        
        public override string ToString()
        {
            return $"CuentaUnificada={cuentaUnificada}" +
                   $"&PageIndex={PageIndex}&PageSize={PageSize}&Active={Active}&Direction={Direction}";
        }

    }
}
