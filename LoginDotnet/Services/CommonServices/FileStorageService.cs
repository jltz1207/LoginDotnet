using System.Linq.Expressions;

namespace LoginDotnet.Services.CommonServices
{
    public class FileStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<FileStorageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContext;
        public FileStorageService(ILogger<FileStorageService> logger, IConfiguration configuration, IWebHostEnvironment environment, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _basePath = configuration["FileStorage:_basePath"] ?? Path.Combine(_environment.ContentRootPath, "Uploads");
            _httpContext = httpContext;
        }

        public async Task<string> SaveFileAsync(IFormFile file, List<string>? subDirectories, bool isReplace = false)
        {
            try
            {
                var allPaths = new List<string> { _basePath };
                if (subDirectories != null && subDirectories.Any())
                {
                    allPaths.AddRange(subDirectories);
                }

                var targetDirectory = Path.Combine(allPaths.ToArray());
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                if (isReplace)
                {
                    DeleteAllFilesInFolder(targetDirectory);
                }
                var uniqueFileName = $"{Guid.NewGuid()}${Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(targetDirectory, uniqueFileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var relativeElements = new List<string> { "Uploads" };
                relativeElements.AddRange(subDirectories);
                relativeElements.Add(uniqueFileName);
                var relativeFilePath = Path.Combine(relativeElements.ToArray());

                return relativeFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                throw;
            }
        }

        public bool DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public void DeleteAllFilesInFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    _logger.LogWarning("Folder does not exist: {FolderPath}", folderPath);
                    return;
                }

                // Get all files
                var files = Directory.GetFiles(folderPath);

                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        _logger.LogDebug("Deleted file: {FilePath}", file);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete file: {FilePath}", file);
                        // Continue with other files
                    }
                }

                _logger.LogInformation("Deleted {FileCount} files from {FolderPath}", files.Length, folderPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting files from folder: {FolderPath}", folderPath);
                throw;
            }
        }
        public string GenerateFileLink(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                {
                    throw new Exception("relativePath is null or empty");
                }

                // cleaning path
                relativePath = relativePath.Replace("\\", "/");

                relativePath = GenerateLocalLink(relativePath);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating file link for: {RelativePath}", relativePath);
                throw ex;
            }

        }

       
        public string GenerateLocalLink(string relativePath)
        {
            var httpContext = _httpContext.HttpContext;

            if (httpContext != null)
            {
                // Use current request context
                var request = httpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // Check if we need to prepend application path
                var appBasePath = httpContext.Request.PathBase;
                if (!string.IsNullOrEmpty(appBasePath))
                {
                    return $"{baseUrl}{appBasePath}/{relativePath}";
                }

                return $"{baseUrl}/{relativePath}";
            }
            else
            {
                // Fallback for background services/console apps
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7252";
                return $"{baseUrl.TrimEnd('/')}/{relativePath}";
            }
        }
    }

}
