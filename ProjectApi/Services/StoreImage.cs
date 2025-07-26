using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Text;

namespace ProjectApi.Services
{
    public class StoreImage
    {

        public  async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "/app/ProjectApi/uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await imageFile.CopyToAsync(stream);
            //}
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageFile.OpenReadStream()))
            {
                // تغيير حجم الصورة (اختياري حسب حاجتك)
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(1024, 1024) 
                }));

                // إعدادات الضغط
                var encoder = new JpegEncoder
                {
                    Quality = 70 
                };

                await image.SaveAsJpegAsync(filePath, encoder);
            }
            return fileName; 
        }

    }
}
