using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport;

namespace FileUploadSample
{
	public static class FileUploadHelper
	{
		public static async Task<FileUploadCompletionNotification> UploadFileAsync(string filePath, DeviceClient deviceClient)
		{
			using var fileStreamSource = new FileStream(filePath, FileMode.Open);
			var fileUploadSasUriRequest = new FileUploadSasUriRequest
			{
				BlobName = Path.GetFileName(fileStreamSource.Name)
			};
			var sasUri = await deviceClient.GetFileUploadSasUriAsync(fileUploadSasUriRequest);
			var fileUploadCompletionNotification = new FileUploadCompletionNotification
			{
				CorrelationId = sasUri.CorrelationId
			};
			try
			{
				var blockBlobClient = new BlockBlobClient(sasUri.GetBlobUri());
				await blockBlobClient.UploadAsync(fileStreamSource, new BlobUploadOptions());
				fileUploadCompletionNotification.IsSuccess = true;
				fileUploadCompletionNotification.StatusCode = (int)HttpStatusCode.OK;
				fileUploadCompletionNotification.StatusDescription = "Success";
			}
			catch (Exception ex)
			{
				fileUploadCompletionNotification.IsSuccess = false;
				fileUploadCompletionNotification.StatusCode = (int)HttpStatusCode.InternalServerError;
				fileUploadCompletionNotification.StatusDescription = ex.Message;
			}
			await deviceClient.CompleteFileUploadAsync(fileUploadCompletionNotification);
			return fileUploadCompletionNotification;
		}
	}
}