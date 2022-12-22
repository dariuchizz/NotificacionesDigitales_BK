using Common.Validations;

namespace Common.Model.Dto
{
    public class CsvCampaniaDto
    {
        public long IdCsvCampania { get; set; }
        public long IdCanal { get; set; }
        public string Dato { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreApellido { get; set; }
        public string Domicilio { get; set; }
        public long Secuencia { get; set; }         

        public static CsvCampaniaDto FromCsv(string csvLine, int canalCampaniaSeleccion)
        {
            string[] values = csvLine.Split(';');
            CsvCampaniaDto item = new CsvCampaniaDto();
            item.IdCanal = canalCampaniaSeleccion;
            item.Dato = values[0];
            item.Nombre = values[1];
            item.Apellido = values[2];
            item.NombreApellido = values[3];
            item.Domicilio = values[4];
            return item;
        }
    }
}
