using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder.IoT
{
    public class UploadedFile
    {
        public UploadedFile(string title, byte[] file)
        {
            File = file;
            Title = title;

            FileName = $"{title}-{DateTime.UtcNow.Ticks}.mp4";
        }
        public string Title { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
    }
}
