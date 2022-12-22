namespace Common.Model.Request
{
    public class GridEventosAccionesRequest: GridRequest
    {
        public override string ToString()
        {
            return $"&PageIndex={PageIndex}&PageSize={PageSize}&Active={Active}&Direction={Direction}";
        }
    }
}
