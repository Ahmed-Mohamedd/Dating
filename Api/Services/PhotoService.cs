using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            var acc = new Account(cloudinaryConfig.Value.CloudName ,
                                  cloudinaryConfig.Value.ApiKey    ,
                                  cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile File)
        {
            var UploadResult = new ImageUploadResult();
            if(File.Length>0)
            {
                using var stream = File.OpenReadStream();
                var ImageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(File.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity(Gravity.Face),
                    Folder = "da-net7"
                };

                UploadResult = await _cloudinary.UploadAsync(ImageUploadParams);
            }
            return UploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
