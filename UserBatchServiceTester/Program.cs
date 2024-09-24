using Newtonsoft.Json;
using ServerApp.DAL;
using ServerApp.Models;
using ServerApp.Services;


// Test batch service console.
public class Program
{
	private static void Main(string[] args)
	{
		TestBatchService().Wait();
	}
	private async static Task TestBatchService()
	{
		var userBatchService = new UserBatchService();
		await userBatchService.SendDriverUpdatesToAllUsers();
	}
}