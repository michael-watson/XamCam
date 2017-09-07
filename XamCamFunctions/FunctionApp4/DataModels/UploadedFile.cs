using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp4.DataModels
{
    public class UploadedFile
    {

        public byte[] File { get; set; }
        public string FileName { get; set; }

        public string UploadedAt { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }
        public string AccountType { get; set;}

    }
}
