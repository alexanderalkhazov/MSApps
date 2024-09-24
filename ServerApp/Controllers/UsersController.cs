using Newtonsoft.Json;
using ServerApp.DAL;
using ServerApp.Models;
using System.Net;

namespace ServerApp.Controllers
{
	public class UsersController
	{
		private readonly DatabaseConnection _databaseConnection;
        public UsersController()
        {
            _databaseConnection = new DatabaseConnection();
        }
		public async Task<string> GetAll(HttpListenerContext context)
		{
			var users = await this._databaseConnection.GetAllUsers();
			if (!users.Any() || users == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				return "No users present in our services.";
			}
			context.Response.StatusCode = (int)HttpStatusCode.OK;
			return JsonConvert.SerializeObject(users);
		}
		public async Task<string> GetById(int id, HttpListenerContext context)
		{
			var foundUser = await this._databaseConnection.GetUserById(id);
			if (foundUser != null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				return JsonConvert.SerializeObject(foundUser);
			}
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return "User was not found.";
		}
		public async Task<string> Create(User user, HttpListenerContext context)
		{
			if (user == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return "User can't be null";
			}
			if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email))
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return "Bad values were provided in request. please make sure values are not empty.";
			}
			await this._databaseConnection.CreateUser(user);
			context.Response.StatusCode = (int)HttpStatusCode.Created;
			return "Successfully created and saved user";
        }
		public async Task<string> Update(int id, User updatePayload, HttpListenerContext context)
		{
			if (updatePayload == null)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return "User can't be null";
			}
			if (string.IsNullOrEmpty(updatePayload.Name) || string.IsNullOrEmpty(updatePayload.Password) || string.IsNullOrEmpty(updatePayload.Email))
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				return "Bad values were provided in request. please make sure values are not empty.";
			}
			await this._databaseConnection.UpdateUser(id ,updatePayload);
			context.Response.StatusCode = (int)HttpStatusCode.Created;
			return "Successfully updated and saved user";
		}
		public async Task<string> Delete(int id, HttpListenerContext context)
		{
			var foundUser = await this._databaseConnection.GetUserById(id);
			if (foundUser != null)
			{
				await this._databaseConnection.DeleteUser(id);
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				return "Deleted user.";
			}
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			return "User not found";
		}
	}
}
