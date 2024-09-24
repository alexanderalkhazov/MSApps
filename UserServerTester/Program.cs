using Newtonsoft.Json;
using ServerApp.DAL;
using ServerApp.Models;
using System.Text;

// HttpClient in console to test server endpoint.
internal class Program
{
	private static string Server_URL = "http://localhost:8080";

	private static void Main(string[] args)
	{
        // Wait for the server to initialize.
        Console.WriteLine("Waititing for server to initialize");
        Thread.Sleep(7000);
		// Test all methods.
		TestGetAllUsers().Wait();
		TestSpecificUser().Wait();
		TestCreateUser().Wait();
		TestUpdateUser().Wait();
		TestDeleteUser().Wait();
		Console.ReadLine();
	}

	private static async Task TestGetAllUsers()
	{
		try
		{
			Console.WriteLine("Hitting GetUsers endpoint.");
			HttpClient httpClient = new HttpClient();
			var response = await httpClient.GetAsync($"{Server_URL}/api/users");
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully got all users!");
				var stringJson = await response.Content.ReadAsStringAsync();
				Console.WriteLine(stringJson);
				var entities = JsonConvert.DeserializeObject<IEnumerable<User>>(stringJson);
			}
			else
			{
				Console.WriteLine("Something went wrong getting users from server.");
			}
		}
		catch (Exception ex) 
		{
			Console.WriteLine(ex.Message);
		}
	}

	private static async Task TestSpecificUser()
	{
		try
		{
			Console.WriteLine("Hitting GetUser endpoint.");
			HttpClient httpClient = new HttpClient();
			var response = await httpClient.GetAsync($"{Server_URL}/api/users/getUser/2");
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully got user!");
				var stringJson = await response.Content.ReadAsStringAsync();
				Console.WriteLine(stringJson);
				var entity = JsonConvert.DeserializeObject<User>(stringJson);
			}
			else
			{
				Console.WriteLine("Something went wrong getting users from server.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
	private static async Task TestCreateUser()
	{
		try
		{
			Console.WriteLine("Hitting CreateUser endpoint.");
			HttpClient httpClient = new HttpClient();

			var someNewUser = new User { Name = "TestUser", Email = "Test@gmail.com", Password = "QWERTY@@1" };
			var json = JsonConvert.SerializeObject(someNewUser);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await httpClient.PostAsync($"{Server_URL}/api/users", content);
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully created user!");
			}
			else
			{
				Console.WriteLine("Something went wrong creating user.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private static async Task TestUpdateUser()
	{
		try
		{
			Console.WriteLine("Hitting UpdateUser endpoint.");
			HttpClient httpClient = new HttpClient();

			var someNewUserUpdates = new User { Name = "TestUserupdated", Email = "Testupdated@gmail.com", Password = "QWERTY@@1222" };
			var json = JsonConvert.SerializeObject(someNewUserUpdates);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await httpClient.PutAsync($"{Server_URL}/api/users/updateUser/1", content);
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully updated user!");
			}
			else
			{
				Console.WriteLine("Something went wrong updating user.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private static async Task TestDeleteUser()
	{
		try
		{
			Console.WriteLine("Hitting DeleteUser endpoint.");
			HttpClient httpClient = new HttpClient();
			var response = await httpClient.DeleteAsync($"{Server_URL}/api/users/deleteUser/2");
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Successfully deleted user!");
			}
			else
			{
				Console.WriteLine("Something went wrong deleting user.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}