
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XamCamFunctions.DataModels;

namespace XamCamFunctions.CosmosDB
{
    static class CosmosDBService
    {
        //DBS - Collections - Documents
        static readonly string DatabaseId = "XamCam";
        static readonly string CollectionId = "XamCamAccounts3";

        //CLIENT
        static readonly DocumentClient myDocumentClient = new DocumentClient(new Uri(Constants.CosmosDBEndPoint), Constants.CosmosDBMyKey);


        //GETALL
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsAsync()
        {

            var listOfAccounts = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient.CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                                            .AsDocumentQuery();
                while (query.HasMoreResults)
                {
                    listOfAccounts.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return listOfAccounts;
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsByIdAsync(string id)
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
        public static async Task<MediaAssetsWithMetaData> GetMediaFileByFileNameAsync(string inputFilename)
        {
            var listOfAccountsByFilename = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient.CreateDocumentQuery<MediaAssetsWithMetaData>
                    (UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f => f.fileName == inputFilename)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    listOfAccountsByFilename.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return listOfAccountsByFilename.FirstOrDefault();
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsByEmailAsync(string inputEmail)
        {
            var myListOfAccountsByEmail = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient.CreateDocumentQuery<MediaAssetsWithMetaData>
                    (UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f => f.email == inputEmail)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    myListOfAccountsByEmail.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
                }
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }
            return myListOfAccountsByEmail;
        }

        //POST
        public static async Task PostMediaAssetAsync(MediaAssetsWithMetaData aMediaAssetsWithMetaData)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), aMediaAssetsWithMetaData);
        }

        //POST
        public static async Task PostIAssetAsync(IAsset asset)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), asset);
        }

        //PUT
        public static async Task PutMediaAssetAsync(MediaAssetsWithMetaData aMediaAssetsWithMetaData)
        {
            await myDocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, aMediaAssetsWithMetaData.id), aMediaAssetsWithMetaData);
        }

        //DELETE
        public static async Task DeleteMediaAssetAsync(MediaAssetsWithMetaData deleteMediaAssetsWithMetaData)
        {
            await myDocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, deleteMediaAssetsWithMetaData.id));
        }
    }
}