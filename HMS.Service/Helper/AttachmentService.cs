using HMS.ServiceAbstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service.Helper
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string[] extensions = { ".jpg", ".jpeg", ".png" };
        private readonly long maxFileSize = 5 * 1024 * 1024; // 5 MB
        public async Task<string?> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                // 1. check if file is null or empty
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("File is null or empty"); // ADD LOGGING
                    return null;
                }

                // 2. check file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!extensions.Contains(fileExtension))
                {
                    Console.WriteLine($"Invalid extension: {fileExtension}"); // ADD LOGGING
                    return null;
                }

                // 3. check file size
                if (file.Length > maxFileSize)
                {
                    Console.WriteLine($"File too large: {file.Length} bytes"); // ADD LOGGING
                    return null;
                }

                // Rest of your code...
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}"); // ADD LOGGING
                return null;
            }
        }
        public bool Delete(string folderName, string fileName)
        {
            if (folderName == null || fileName == null)
                return false;

            // 1. Get Located Folder Path
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", folderName, fileName);

            // 2. Check If File Exists
            if (!File.Exists(fullPath))
                return false;

            // 3. Delete The File
            File.Delete(fullPath);
            return true;
        }

    }
}
