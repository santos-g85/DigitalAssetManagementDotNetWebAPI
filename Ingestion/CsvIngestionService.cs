using System.Dynamic;
using CsvHelper;
using System.Globalization;

namespace DAMApi.Ingestion
{
    public class CsvIngestionService
    {
        public async Task<List<dynamic>> IngestCsvAsync(Stream csvstream)
        {
            var record = new List<dynamic>();  
            using var reader = new StreamReader(csvstream); 
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture) ;
            var dynamicRecords = csv.GetRecords<dynamic>().ToList();
            return dynamicRecords;
        }
    }
}
