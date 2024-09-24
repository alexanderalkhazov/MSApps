using Quartz;
using Quartz.Impl;
using ServerApp.DAL;

namespace ServerApp.Services
{
	public class UserBatchService
	{
		private readonly DatabaseConnection _databaseConnection;
		public UserBatchService()
		{
			this._databaseConnection = new DatabaseConnection();
		}
		public async Task SendDriverUpdatesToAllUsers()
		{
			var schedulerFactory = new StdSchedulerFactory();
			var scheduler = await schedulerFactory.GetScheduler();
			await scheduler.Start();


			var jobData = new JobDataMap
			{
				{ "databaseConnection", this._databaseConnection }
			};

			var job = JobBuilder.Create<UserBatchJob>()
				.WithIdentity("userBatchJob", "group1")
				.UsingJobData(jobData)
				.Build();

			var trigger = TriggerBuilder.Create()
				.WithIdentity("userBatchTrigger", "group1")
				.StartNow()
				.WithSimpleSchedule(x => x
					.WithIntervalInSeconds(300) 
					.RepeatForever())          
				.Build();

			await scheduler.ScheduleJob(job, trigger);

			Console.WriteLine("Batch service scheduled. Press Enter to exit.");
			Console.ReadLine();

			await scheduler.Shutdown();
		}
	}
	public class UserBatchJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			var databaseConnection = (DatabaseConnection)context.JobDetail.JobDataMap.First(x => x.Key == "databaseConnection").Value;

			var allUsers = await databaseConnection.GetAllUsers();
			if (allUsers == null || !allUsers.Any())
			{
				Console.WriteLine("No users in database.");
				return;
			}
			foreach (var user in allUsers)
			{
				Console.WriteLine($"Sending notifaction for user to update his Computer drivers. UserID: {user.ID} Username: {user.Name}\tuser's email address {user.Email}");
            }

			await Task.CompletedTask;
		}
	}
}
