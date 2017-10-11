using System;

namespace XamCamFunctions.DataModels
{
    public class UploadedFile
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string AccountType { get; set; }
    }
}
