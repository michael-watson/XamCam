using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;

using Microsoft.WindowsAzure.MediaServices.Client;

using XamCam.Functions.Models;

namespace XamCam.Functions
{
    static class CosmosDBService
    {
        //DBS - Collections - Documents
        static readonly string DatabaseId = "XamCam";
        static readonly string CollectionId = "XamCamAccounts3";

        //CLIENT
        static readonly DocumentClient myDocumentClient = new DocumentClient(new Uri(Constants.CosmosDBEndPoint), Constants.CosmosDBKey);


        //GETALL
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsAsync()
        {
            var mediaAssetList = new List<MediaAssetsWithMetaData>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    mediaAssetList.AddRange(await documentQuery.ExecuteNextAsync<MediaAssetsWithMetaData>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return mediaAssetList;
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsWithMediaAssetUrl()
        {
            var mediaAssetWithUrlList = new List<MediaAssetsWithMetaData>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(x => x.mediaAssetUri != null)
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    mediaAssetWithUrlList.AddRange(await documentQuery.ExecuteNextAsync<MediaAssetsWithMetaData>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return mediaAssetWithUrlList;
        }


        //GET
        public static async Task<List<Document>> GetAllMediaAssetsByIdAsync(string id)
        {
            var resourceResponse = await myDocumentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));

            if (resourceResponse.StatusCode != System.Net.HttpStatusCode.OK)
                return null;

            var mediaAssetList = new List<Document> { resourceResponse };

            return mediaAssetList;
        }

        //GET
        public static async Task<MediaAssetsWithMetaData> GetMediaFileByFileNameAsync(string inputFilename)
        {
            var fileNameMediaAssetList = new List<MediaAssetsWithMetaData>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(x => x.fileName == inputFilename)
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    fileNameMediaAssetList.AddRange(await documentQuery.ExecuteNextAsync<MediaAssetsWithMetaData>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return fileNameMediaAssetList.FirstOrDefault();
        }

        //GET
        public static async Task<List<MediaAssetsWithMetaData>> GetAllMediaAssetsByEmailAsync(string inputEmail)
        {
            var emailMediaAssetList = new List<MediaAssetsWithMetaData>();
            try
            {
                var query = myDocumentClient
                    .CreateDocumentQuery<MediaAssetsWithMetaData>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f => f.email == inputEmail)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                    emailMediaAssetList.AddRange(await query.ExecuteNextAsync<MediaAssetsWithMetaData>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return emailMediaAssetList;
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