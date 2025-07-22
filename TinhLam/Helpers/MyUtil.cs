using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace TinhLam.Helpers
{
    public class MyUtil
    {
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhnujmikolpQAZWSXEDCRFVTGBYHNUJMIKOLP!";
            var sb = new StringBuilder();
            var rd = new Random();
            for(int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Tạo thumbnail cho ảnh, lưu vào đường dẫn chỉ định
        /// </summary>
        /// <param name="inputPath">Đường dẫn ảnh gốc</param>
        /// <param name="outputPath">Đường dẫn lưu thumbnail</param>
        /// <param name="maxWidth">Chiều rộng tối đa</param>
        /// <param name="maxHeight">Chiều cao tối đa</param>
        public static void CreateThumbnail(string inputPath, string outputPath, int maxWidth = 300, int maxHeight = 300)
        {
            using (var image = Image.Load(inputPath))
            {
                var ratioX = (double)maxWidth / image.Width;
                var ratioY = (double)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);
                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);
                image.Mutate(x => x.Resize(newWidth, newHeight));
                image.Save(outputPath);
            }
        }

        /// <summary>
        /// Upload ảnh lên Cloudinary, trả về URL ảnh
        /// </summary>
        public static string UploadImageToCloudinary(IFormFile file, IConfiguration configuration, string folder = "images")
        {
            try
            {
                var cloudName = configuration["Cloudinary:CloudName"];
                var apiKey = configuration["Cloudinary:ApiKey"];
                var apiSecret = configuration["Cloudinary:ApiSecret"];
                var account = new Account(cloudName, apiKey, apiSecret);
                var cloudinary = new Cloudinary(account);

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = folder
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                    {
                        return uploadResult.SecureUrl.ToString();
                    }
                    else
                    {
                        Console.WriteLine($"Cloudinary upload failed: {uploadResult.Error?.Message}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cloudinary upload error: " + ex.Message);
                return null;
            }
        }
    }
}
