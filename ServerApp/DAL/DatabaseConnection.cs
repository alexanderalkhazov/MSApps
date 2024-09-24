using MySql.Data.MySqlClient;
using ServerApp.Models;
using System.Data;
using System.Text;

namespace ServerApp.DAL
{
	public class DatabaseConnection
	{
        private readonly string _connectionString;
		private readonly string _databaseName;
		private readonly string _usersTableName;
		public DatabaseConnection()
        {
            this._connectionString = "Server=localhost;User ID=root;Password=qwerty;";
			this._databaseName = "my_db";
			this._usersTableName = "Users";
			Console.WriteLine("Initializng database and table if they don't exist yet.");
			this.InitializeDatabaseAndTable().Wait();
		}

		private async Task InitializeDatabaseAndTable()
		{
			using (var connection = new MySqlConnection(this._connectionString))
			{
				try
				{
					await connection.OpenAsync();
					Console.WriteLine("Connection to MySQL server successful!");

					string checkDbSql = $"SHOW DATABASES LIKE '{this._databaseName}';";
					using (var checkDbCommand = new MySqlCommand(checkDbSql, connection))
					{
						var dbExists = await checkDbCommand.ExecuteScalarAsync();

						if (dbExists == null)
						{
							string createDbSql = $"CREATE DATABASE {this._databaseName};";
							using (var createDbCommand = new MySqlCommand(createDbSql, connection))
							{
								await createDbCommand.ExecuteNonQueryAsync();
								Console.WriteLine($"Database '{this._databaseName}' created successfully.");
							}
						}
						else
						{
							Console.WriteLine($"Database '{this._databaseName}' already exists.");
						}
					}

					connection.ChangeDatabase(this._databaseName);

					string checkTableSql = "SHOW TABLES LIKE 'Users';";
					using (var checkCommand = new MySqlCommand(checkTableSql, connection))
					{
						var tableExists = await checkCommand.ExecuteScalarAsync();

						if (tableExists == null)
						{
							string createTableSql = @"
                            CREATE TABLE Users (
                                ID INT AUTO_INCREMENT PRIMARY KEY,
                                Name VARCHAR(255),
                                Email VARCHAR(255),
                                Password VARCHAR(255)
                            );";

							using (var createCommand = new MySqlCommand(createTableSql, connection))
							{
								await createCommand.ExecuteNonQueryAsync();
								Console.WriteLine("Users table created successfully.");
							}
						}
						else
						{
							Console.WriteLine("Users table already exists.");
						}
					}
				}
				catch (MySqlException ex)
				{
					Console.WriteLine($"MySQL error: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}
			}
		}

		public async Task<IEnumerable<User>> GetAllUsers()
		{
			var users = new List<User>();

			using (var connection = new MySqlConnection(this._connectionString))
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(this._databaseName);

				string sql = $"SELECT * FROM {this._usersTableName};";
				using (var command = new MySqlCommand(sql, connection))
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						users.Add(new User
						{
							ID = reader.GetInt32("ID"),
							Name = reader.GetString("Name"),
							Email = reader.GetString("Email"),
							Password = reader.GetString("Password"),
						});
					}
				}
			}

			return users;
		}

		public async Task<User> GetUserById(int id)
		{
			User user = null;

			using (var connection = new MySqlConnection(this._connectionString))
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(this._databaseName);

				string sql = $"SELECT * FROM {this._usersTableName} WHERE ID = @id;";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							user = new User
							{
								ID = reader.GetInt32("ID"),
								Name = reader.GetString("Name"),
								Email = reader.GetString("Email"),
								Password = reader.GetString("Password"),
							};
						}
					}
				}
			}

			return user;
		}

		public async Task CreateUser(User user)
		{
			using (var connection = new MySqlConnection(this._connectionString))
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(this._databaseName);

				string sql = $"INSERT INTO {this._usersTableName} (Name, Email, Password) VALUES (@name, @email, @password);";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", user.Name);
					command.Parameters.AddWithValue("@email", user.Email);
					command.Parameters.AddWithValue("@password", user.Password);
					await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task UpdateUser(int id, User user)
		{
			using (var connection = new MySqlConnection(this._connectionString))
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(this._databaseName);

				string sql = $"UPDATE {this._usersTableName} SET Name = @name, Email = @email, Password = @password WHERE ID = @id;";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", user.Name);
					command.Parameters.AddWithValue("@email", user.Email);
					command.Parameters.AddWithValue("@password", user.Password);
					command.Parameters.AddWithValue("@id", id);
					await command.ExecuteNonQueryAsync();
				}
			}
		}

		public async Task DeleteUser(int id)
		{
			using (var connection = new MySqlConnection(this._connectionString))
			{
				await connection.OpenAsync();
				connection.ChangeDatabase(this._databaseName);

				string sql = $"DELETE FROM {this._usersTableName} WHERE ID = @id;";
				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					await command.ExecuteNonQueryAsync();
				}
			}
		}
	}
}
