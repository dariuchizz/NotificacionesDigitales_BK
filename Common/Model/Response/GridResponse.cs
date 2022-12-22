using System.Collections.Generic;

namespace Common.Model.Response
{
    public class GridResponse<T>
    {
        public long Cantidad { get; set; }

        public IEnumerable<T> Grid { get; set; }
    }
}
