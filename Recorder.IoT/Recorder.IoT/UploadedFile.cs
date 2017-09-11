using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Recorder.IoT
{
    public class UploadedFile
    {
        public UploadedFile(string title, byte[] file)
        {
            File = file;
            Title = title;

            FileName = $"{DeviceInfo.Instance.Id}-{DateTime.UtcNow.Ticks}.wmv";
        }

        public string Title { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
    }
}