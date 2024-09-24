using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerApp.Controllers;
using ServerApp.Models;

public class Server
{
	private static readonly HttpListener listener = new HttpListener();
	private readonly UsersController _userController;

	public Server(string uriPrefix)
	{
		listener.Prefixes.Add(uriPrefix);
		this._userController = new UsersController();
	}

	public void Start()
	{
		listener.Start();
		Console.WriteLine("Listening for connections...");
		Task.Run(async () => await ListenForRequests());
	}

	public void Stop()
	{
		listener.Stop();
		listener.Close();
		Console.WriteLine("Server stopped.");
	}

	private async Task ListenForRequests()
	{
		while (true)
		{
			var context = await listener.GetContextAsync();
			_ = HandleRequest(context); // Fire and forget
		}
	}

	private async Task HandleRequest(HttpListenerContext context)
	{
		string responseString = "";
		context.Response.ContentType = "application/json";

		try
		{
			var method = context.Request.HttpMethod;
			var path = context.Request.Url.AbsolutePath;

			if (method == "GET" && path == "/api/users")
			{
				responseString = await _userController.GetAll(context);
			}
			else if (method == "POST" && path == "/api/users")
			{
				using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
				{
					var body = await reader.ReadToEndAsync();
					var user = JsonConvert.DeserializeObject<User>(body);
					responseString = await this._userController.Create(user, context); ;
				}
			}
			else if (method == "GET" && path.StartsWith("/api/users/getUser/"))
			{
				int id = int.Parse(path.Split('/')[4]);
				responseString = await _userController.GetById(id, context);
			}
			else if (method == "PUT" && path.StartsWith("/api/users/updateUser/"))
			{
				using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
				{
					int id = int.Parse(path.Split('/')[4]);
					var body = await reader.ReadToEndAsync();
					var user = JsonConvert.DeserializeObject<User>(body);
					responseString = await _userController.Update(id, user, context);
				}
			}
			else if (method == "DELETE" && path.StartsWith("/api/users/deleteUser"))
			{
				int id = int.Parse(path.Split('/')[4]);
				responseString = await _userController.Delete(id, context);
			}
			else
			{
				responseString = "This type of method is not supported by our services.";
			}
		}
		catch (Exception ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			responseString = JsonConvert.SerializeObject(new { error = ex.Message });
		}

		using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
		{
			await writer.WriteAsync(responseString);
		}

		context.Response.Close();
	}
}
