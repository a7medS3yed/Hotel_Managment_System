using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ServiceAbstraction
{
    public interface IAttachmentService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folderName);
        bool Delete(string folderName, string fileName);
    }
}
