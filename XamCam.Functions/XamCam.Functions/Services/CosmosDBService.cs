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
        public static async Task<List<MediaMetadata>> GetAllMediaAssetsAsync()
        {
            var mediaAssetList = new List<MediaMetadata>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaMetadata>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    mediaAssetList.AddRange(await documentQuery.ExecuteNextAsync<MediaMetadata>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return mediaAssetList;
        }

        //GET
        public static async Task<List<MediaMetadata>> GetAllMediaAssetsWithMediaAssetUrl()
        {
            var mediaAssetWithUrlList = new List<MediaMetadata>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaMetadata>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(x => x.MediaAssetUri != null)
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    mediaAssetWithUrlList.AddRange(await documentQuery.ExecuteNextAsync<MediaMetadata>());
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
        public static async Task<MediaMetadata> GetMediaFileByFileNameAsync(string inputFilename)
        {
            var fileNameMediaAssetList = new List<MediaMetadata>();

            try
            {
                var documentQuery = myDocumentClient
                    .CreateDocumentQuery<MediaMetadata>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(x => x.FileName == inputFilename)
                    .AsDocumentQuery();

                while (documentQuery.HasMoreResults)
                    fileNameMediaAssetList.AddRange(await documentQuery.ExecuteNextAsync<MediaMetadata>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return fileNameMediaAssetList.FirstOrDefault();
        }

        //GET
        public static async Task<List<MediaMetadata>> GetAllMediaAssetsByEmailAsync(string inputEmail)
        {
            var emailMediaAssetList = new List<MediaMetadata>();
            try
            {
                var query = myDocumentClient
                    .CreateDocumentQuery<MediaMetadata>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                    .Where(f => f.Email == inputEmail)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                    emailMediaAssetList.AddRange(await query.ExecuteNextAsync<MediaMetadata>());
            }
            catch (DocumentClientException ex)
            {
                Debug.WriteLine("Error: ", ex.Message);
            }

            return emailMediaAssetList;
        }

        //POST
        public static async Task PostMediaAssetAsync(MediaMetadata aMediaAssetsWithMetaData)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), aMediaAssetsWithMetaData);
        }

        //POST
        public static async Task PostIAssetAsync(IAsset asset)
        {
            await myDocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), asset);
        }

        //PUT
        public static async Task PutMediaAssetAsync(MediaMetadata aMediaAssetsWithMetaData)
        {
            await myDocumentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, aMediaAssetsWithMetaData.Id), aMediaAssetsWithMetaData);
        }

        //DELETE
        public static async Task DeleteMediaAssetAsync(MediaMetadata deleteMediaAssetsWithMetaData)
        {
            await myDocumentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, deleteMediaAssetsWithMetaData.Id));
        }
    }
}