using DAMApi.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAMApi.Services.Implementation;

namespace DAMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly GoogleApiService _googleApiService;

        public FolderController(GoogleApiService googleApiService)
        {
            _googleApiService = googleApiService ??
                throw new ArgumentNullException(nameof(googleApiService), "Google API service cannot be null.");
        }

        [HttpPost("CreateFolderTree")]
        public async Task<IActionResult> CreateFolderTree(string FolderName)
        {
            var result = await _googleApiService.CreateFolderTree(FolderName);

            if (result is null)
            {
                return BadRequest("Failed to create folder tree.");
            }

            return Ok(new { Message = "Folder tree created successfully.", Data = result });
        }
    }
}
