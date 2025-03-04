using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;

namespace FileUploadSample
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("launchSettings.json", true, true)
				.AddEnvironmentVariables()
				.Build();
			using var deviceClient = DeviceClient.CreateFromConnectionString(configuration["IOTHUB_DEVICE_CONN_STRING"]);
			var result = await FileUploadHelper.UploadFileAsync("scenery.jpg", deviceClient);
			Console.WriteLine($"File upload result: {JsonSerializer.Serialize(result)}");
			await deviceClient.CloseAsync();
		}
	}
}