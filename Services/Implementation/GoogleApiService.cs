using DAMApi.Models.Enums;
using DAMApi.Settings;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DAMApi.Services.Implementation
{
    public class GoogleApiService
    {
        private readonly GoogleApiSettings _googleApiSettings;
        public GoogleApiService(IOptions<GoogleApiSettings> options)
        {
            _googleApiSettings = options.Value ?? 
                throw new ArgumentNullException(nameof(options), "Google API settings cannot be null."); 
        }
        private DriveService CreateDriveService()
        {
            var token = new TokenResponse
            {
                RefreshToken = _googleApiSettings.RefreshToken,
            };

            var credentials = new UserCredential
                (
                new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _googleApiSettings.ClientId,
                        ClientSecret = _googleApiSettings.ClientSecret
                    },
                    Scopes = new[] { DriveService.Scope.Drive }
                }),
                "user",
                token
                );
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "DAMAPI"
            });
            return service;
        }
        private static async Task<Google.Apis.Drive.v3.Data.File> CreateFolder(string FolderName, 
            DriveService service)
        {
            //create parent folder
            var parentMetaData = new Google.Apis.Drive.v3.Data.File
            {
                Name = FolderName,
                MimeType = "application/vnd.google-apps.folder"
            };

            var ParentCreateRequest = service.Files.Create(parentMetaData);
            ParentCreateRequest.Fields = "id,name";

            var parentFolder = await ParentCreateRequest.ExecuteAsync();
            return parentFolder;
        }

        public async Task<IActionResult> CreateFolderTree(string FolderName)
        {
            DriveService service = CreateDriveService();
            try
            {
                //create parent folder
                Google.Apis.Drive.v3.Data.File parentFolder = await CreateFolder(FolderName, service);

                var subFoldersList = Enum.GetValues<SubFolderTypes>().ToList();

                var subFoldersResult = new List<object>();

                if (parentFolder is null)
                {
                    return null;

                }

                //else create subfolders
                foreach (var subFolderType in subFoldersList)
                {
                    var subMetadata = new Google.Apis.Drive.v3.Data.File
                    {
                        Name = subFolderType.ToString(),
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new List<string> { parentFolder.Id }
                    };

                    var subRequest = service.Files.Create(subMetadata);
                    subRequest.Fields = "id,name";
                    var subResult = await subRequest.ExecuteAsync();

                    subFoldersResult.Add(new
                    {
                        subResult.Id,
                        subResult.Name,
                        ParentId = parentFolder.Id
                    });

                }
                return new OkObjectResult(new
                {
                    ParentFolder = new { parentFolder.Id, parentFolder.Name },
                    SubFolders = subFoldersResult
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message); 
            }
        }

        public async Task<IActionResult> ListFiles()
        {
            DriveService service = CreateDriveService();

            var request = service.Files.List();
            request.Fields = "files(id, name)";
            //request.PageSize = 10;

            var result = await request.ExecuteAsync();

            var files = result.Files.Select(f => new { f.Id, f.Name });

            return new OkObjectResult(files);
        }
    }
}
