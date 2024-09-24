

using ServerApp.DAL;
using ServerApp.Models;

internal class Program
{
	private static void Main(string[] args)
	{
		try
		{
			var server = new Server("http://localhost:8080/");
			server.Start();

			Console.WriteLine("Press any key to stop the server...");
			Console.ReadKey();

			server.Stop();
		}
		catch(Exception ex)
		{
            Console.WriteLine(ex.Message);
        }
	}
}