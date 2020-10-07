using System;
using System.Collections.Generic;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DataHubFileService.Models;
using System.Threading.Tasks;

namespace DataHubFileService
{
    public class S3Access
    {
        public static async Task<List<DataHubFile>> GetFileList(string sourcePath)
        {
            List<DataHubFile> fileList = new List<DataHubFile>();

            using (var client = new AmazonS3Client(RegionEndpoint.USEast1))
            {
                var response = await client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = "maxcapdatahub-dev",
                    Prefix = sourcePath
                });

                foreach (var file in response.S3Objects)
                {
                    if ((file.Key != sourcePath) && FileInDirectory(sourcePath, file.Key))
                    {
					    fileList.Add(new DataHubFile {
                             Path = file.Key,
                             ObjectType = (file.Key.EndsWith("/")) ? "Folder" : "File",
                             DisplayName = GetDisplayName(file.Key)
                        });
                    }
                }
            }

            SortFileList(fileList);
            return fileList;
        }

        public static async Task<string> UploadFile(string destPath, System.IO.Stream stream)
        {
            string result = string.Empty;
            using (var client = new AmazonS3Client(RegionEndpoint.USEast1))
            {
                var request = new UploadPartRequest 
                {
                    BucketName = "maxcapdatahub-dev",
                    Key = destPath,
                    InputStream = stream
                };

                var response = await client.UploadPartAsync(request);
                //result = response.HttpStatusCode.ToString();
            }
            return result;
        }

        public static async Task<string> DeleteFile(string path)
        {
            string result = string.Empty;
            using (var client = new AmazonS3Client(RegionEndpoint.USEast1))
            {
                var request = new DeleteObjectRequest 
                {
                    BucketName = "maxcapdatahub-dev",
                    Key = path
                };

                var response = await client.DeleteObjectAsync(request);
            }
            return result;
        }

        public static void SortFileList(List<DataHubFile> fileList)
        {
            List<DataHubFile> folders = new List<DataHubFile>();
            List<DataHubFile> files = new List<DataHubFile>();

            folders = fileList.FindAll(f => f.Path.EndsWith("/"));
            files = fileList.FindAll(f => !f.Path.EndsWith("/"));

            folders.Sort(DataHubFile.CompareFiles);
            files.Sort(DataHubFile.CompareFiles);

            fileList.Clear();
            fileList.AddRange(folders);
            fileList.AddRange(files);
        }

        public static bool FileInDirectory(string sourcePath, string filePath)
        {
            bool fileInDirectory = false;

            string[] sourcePathParts = sourcePath.Split('/');
            int numSourcePathParts = 0;
            if (sourcePathParts.Length > 0)
            {
                if (sourcePathParts[sourcePathParts.Length - 1] == "")
                    numSourcePathParts = sourcePathParts.Length - 1;
                else
                    numSourcePathParts = sourcePathParts.Length;
            }

            string[] filePathParts = filePath.Split('/');
            int numFilePathParts = 0;
            if (filePathParts.Length > 0)
            {
                if (filePathParts[filePathParts.Length - 1] == "")
                    numFilePathParts = filePathParts.Length - 1;
                else
                    numFilePathParts = filePathParts.Length;
            }

            fileInDirectory = (numFilePathParts == numSourcePathParts + 1);

            return fileInDirectory;
        }

        public static string GetDisplayName(string sourcePath)
        {
            string displayName = string.Empty;

            string[] pathParts = sourcePath.Split('/');
            if (sourcePath.EndsWith("/"))
                displayName = pathParts[pathParts.Length - 2];
            else
                displayName = pathParts[pathParts.Length - 1];

            return displayName;
        }
    }
}