
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;

using FunctionApp4.DataModels;

namespace FunctionApp4.CosmosDB
{
    class CosmosDBMediaFiles
    {
        //DBS - Collections - Documents
        static readonly string DatabaseId = "XamCam";
        static readonly string CollectionId = "XamCamAccounts3";

        //CLIENT
        static readonly DocumentClient myDocumentClient = new DocumentClient(new Uri(Constants.CosmosDBEndPoint), Constants.CosmosDBMyKey);

        public static List<MediaAssetsWithMetaData> MyListOfAccounts;
        public static List<MediaAssetsWithMetaData> MyListOfAccountsByEmail;

        //GETALL
        public static async Task<List<MediaAssetsWithMetaData>> GetAllCosmosDogs()
        {
            MyListOfAccounts = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient.CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                                            .AsDocumentQuery();
                while (query.HasMoreResults)
                {
                    MyListOfAccounts.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return MyListOfAccounts;
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetCosmosDogByIdAsync(string id)
        {
            var result = await myDocumentClient.ReadDocumentAsync<MediaAssetsWithMetaData>(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            List<MediaAssetsWithMetaData> returnedListCosmosDog = new List<MediaAssetsWithMetaData>();
            returnedListCosmosDog.Add(result);

            return returnedListCosmosDog;
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetCosmosDogByEmailAsync(string inputEmail)
        {
            MyListOfAccountsByEmail = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient.CreateDocumentQuery<MediaAssetsWithMetaData>
                    (UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f => f.email == inputEmail)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    MyListOfAccountsByEmail.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return MyListOfAccountsByEmail;
        }

        //POST
        public static async Task PostCosmosDogAsync(MediaAssetsWithMetaData aMediaAssetsWithMetaData)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), aMediaAssetsWithMetaData);
        }
        
        //PUT
        public static async Task PutCosmosDogAsync(MediaAssetsWithMetaData aMediaAssetsWithMetaData)
        {
            await myDocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, aMediaAssetsWithMetaData.id), aMediaAssetsWithMetaData);
        }

        //DELETE
        public static async Task DeleteCosmosDogAsync(MediaAssetsWithMetaData deleteMediaAssetsWithMetaData)
        {
            await myDocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, deleteMediaAssetsWithMetaData.id));
        }
    }
}



 