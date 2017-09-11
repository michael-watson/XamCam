//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System;
//using System.Diagnostics;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Linq;
//using Microsoft.Azure.Documents.Client;

//using FunctionApp4;
//using FunctionApp4.DataModels;
////using ASampleApp.Models;

//namespace FunctionApp4.CosmosDB
//{
//    public class CosmosDBService

//    {
//        //DBS - Collections - Documents
//        static readonly string DatabaseId = "XamCam";
//        static readonly string CollectionId = "XamCamAccounts1";

       
//        //CLIENT
//        static readonly DocumentClient myDocumentClient = new DocumentClient(new Uri(Constants.CosmosDBEndPoint), Constants.CosmosDBMyKey);

//        public static List<XamCamAccount> MyListOfAccounts;

//        //GETALL
//        public static async Task<List<XamCamAccount>> GetAllCosmosDogs()
//        {
//            MyListOfAccounts = new List<XamCamAccount>();
//            try
//            {

//                var query = myDocumentClient.CreateDocumentQuery<XamCamAccount>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
//                                            .AsDocumentQuery();
//                while (query.HasMoreResults)
//                {
//                    MyListOfAccounts.AddRange(await query.ExecuteNextAsync<XamCamAccount>());
//                }
//            }
//            catch (DocumentClientException ex)
//            {
//                Debug.WriteLine("Error: ", ex.Message);
//            }
//            return MyListOfAccounts;
//        }

//        //GET
//        public static async Task<List<XamCamAccount>> GetCosmosDogByIdAsync(string id)
//        {
//            var result = await myDocumentClient.ReadDocumentAsync<XamCamAccount>(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

//            if (result.StatusCode != System.Net.HttpStatusCode.OK)
//            {
//                return null;
//            }

//            List<XamCamAccount> returnedListCosmosDog = new List<XamCamAccount>();
//            returnedListCosmosDog.Add(result);

//            return returnedListCosmosDog;

//        }

//        //POST
//        public static async Task PostCosmosDogAsync(XamCamAccount aXamCamAccount)
//        {
//            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), aXamCamAccount);

//        }

//        //PUT
//        public static async Task PutCosmosDogAsync(XamCamAccount aXamCamAccount)
//        {
//            await myDocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, aXamCamAccount.Id), aXamCamAccount);
//        }

//        //DELETE
//        public static async Task DeleteCosmosDogAsync(XamCamAccount deleteXamCamAccount)
//        {
//            await myDocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, deleteXamCamAccount.Id));
//        }
//    } 
//
//}