using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp4.DataModels
{
    public class XamCamAccountTwo
    {
        public string id { get; set; }

        public string email { get; set; }
        
        public string blobContainer { get; set; }
        
        //not sure we will need the below
        public string password { get; set; }

        public string saltedPassword { get; set; }



        //public string Token_type { get; set; }
        //public string Expires_in { get; set; }
        //public string Ext_expires_in { get; set; }
        //public string Expires_on { get; set; }
        //public string Not_before { get; set; }
        //public string Resource { get; set; }
        //public string Access_token { get; set; }
    }
}





