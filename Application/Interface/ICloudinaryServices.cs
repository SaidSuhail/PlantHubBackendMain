﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Interface
{
   public interface ICloudinaryServices
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
