
namespace MoviesApp.Services
{
    public class LocalFileStorage(IWebHostEnvironment environment,
        IHttpContextAccessor httpContext) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if(string.IsNullOrEmpty(route)) { 
                return Task.CompletedTask;
            }

            var fileName = Path.GetFileName(route);
            var fileDirectory = Path.Combine(environment.WebRootPath, container, fileName);

            if(File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(environment.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, fileName);
            using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var scheme = httpContext.HttpContext.Request.Scheme;
            var host = httpContext.HttpContext.Request.Host;
            var urlBase = $"{scheme}://{host}";
            var urlFile = Path.Combine(urlBase, container, fileName).Replace("\\", "/");
            return urlFile;
        }
    }
}
