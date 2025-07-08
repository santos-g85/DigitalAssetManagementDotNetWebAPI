using DAMApi.Ingestion;
using DAMApi.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DAMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly CsvIngestionService _csvIngestionService;
        private readonly ILogger<FileController> _logger;
        private readonly GoogleApiService _googleApiService;
        public FileController(CsvIngestionService csvIngestionService,
            ILogger<FileController> logger,
            GoogleApiService googleApiService)
        {
            _logger = logger;
            _csvIngestionService = csvIngestionService;
            _googleApiService = googleApiService;
        }

        [HttpPost("UploadCsv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("No file uploaded.");
                return BadRequest("No file uploaded.");
            }
            try
            {
                using var stream = file.OpenReadStream();
                var records = await _csvIngestionService.IngestCsvAsync(stream);
                _logger.LogInformation("CSV file processed successfully with {Count} records.", records.Count);
                return Ok(new
                {
                    Count = records.Count.ToString(),
                    Sample = records.Take(2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CSV file.");
                return 
            }
        }

        [HttpGet("GetAllFiles")]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var files = await _googleApiService.ListFiles();

                if (files == null)
                {
                    _logger.LogWarning("No files found.");
                    return NotFound("No files found.");
                }
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving files.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving files.");
            }
        }
    }
}
