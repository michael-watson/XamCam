using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;

using FunctionApp4;
using FunctionApp4.DataModels;
//using ASampleApp.Models;

namespace FunctionApp4.CosmosDB
{
    public class CosmosDBServiceTwo

    {
        //DBS - Collections - Documents
        static readonly string DatabaseId = "XamCam";
        static readonly string CollectionId = "XamCamAccounts2";


        //CLIENT
        static readonly DocumentClient myDocumentClient = new DocumentClient(new Uri(Constants.CosmosDBEndPoint), Constants.CosmosDBMyKey);

        public static List<XamCamAccountTwo> MyListOfAccounts;
        public static List<XamCamAccountTwo> MyListOfAccountsByEmail;

        //GETALL
        public static async Task<List<XamCamAccountTwo>> GetAllCosmosDogs()
        {
            MyListOfAccounts = new List<XamCamAccountTwo>();
            try
            {

                var query = myDocumentClient.CreateDocumentQuery<XamCamAccountTwo>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                                            .AsDocumentQuery();
                while (query.HasMoreResults)
                {
                    MyListOfAccounts.AddRange(await query.ExecuteNextAsync<XamCamAccountTwo>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return MyListOfAccounts;
        }

        //GET
        public static async Task<List<XamCamAccountTwo>> GetCosmosDogByIdAsync(string id)
        {
            var result = await myDocumentClient.ReadDocumentAsync<XamCamAccountTwo>(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            List<XamCamAccountTwo> returnedListCosmosDog = new List<XamCamAccountTwo>();
            returnedListCosmosDog.Add(result);

            return returnedListCosmosDog;

        }

        //GET
        public static async Task<List<XamCamAccountTwo>> GetCosmosDogByEmailAsync(string inputEmail)
        {
            MyListOfAccountsByEmail = new List<XamCamAccountTwo>();
            try
            {

                var query = myDocumentClient.CreateDocumentQuery<XamCamAccountTwo>
                    (UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f=>f.email == inputEmail)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    MyListOfAccountsByEmail.AddRange(await query.ExecuteNextAsync<XamCamAccountTwo>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return MyListOfAccountsByEmail;
        }


        //POST
        public static async Task PostCosmosDogAsync(XamCamAccountTwo aXamCamAccountTwo)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), aXamCamAccountTwo);

        }


        //PUT
        public static async Task PutCosmosDogAsync(XamCamAccountTwo aXamCamAccountTwo)
        {
            await myDocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, aXamCamAccountTwo.id), aXamCamAccountTwo);
        }

        //DELETE
        public static async Task DeleteCosmosDogAsync(XamCamAccountTwo deleteXamCamAccountTwo
            )
        {
            await myDocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, deleteXamCamAccountTwo.id));
        }
    }
}