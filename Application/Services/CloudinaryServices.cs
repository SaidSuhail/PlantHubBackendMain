using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
   public class CloudinaryServices:ICloudinaryServices
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryServices> _logger;

        public CloudinaryServices(IConfiguration configuration, ILogger<CloudinaryServices> logger)
        {
            _logger = logger;

            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];
            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new Exception("Cloudinary configuration is missing or incomplete");
            }
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string>UploadImageAsync(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                _logger.LogError("File is null or empty");
                return null;
            }
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    _logger.LogError($"Cloudinary upload error:{uploadResult.Error.Message}");
                    throw new Exception($"Cloudinary Upload Error:{uploadResult.Error.Message}");
                }

                _logger.LogInformation($"Cloudinary upload successfull. URL:{uploadResult.SecureUri}");
                return uploadResult.SecureUri?.ToString();
            }
        }
    }
}
