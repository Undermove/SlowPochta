﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using SlowPochta.Business.Module.Configuration;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module
{
	public class MessageStatusUpdater : IDisposable
	{
		private readonly IScheduler _scheduler;
		private readonly DataContext _dataContext;
		private readonly MessageModule _messageModule;
		private readonly MessageStatusUpdaterConfig _config;

		public MessageStatusUpdater(DesignTimeDbContextFactory context, MessageModule messageModule, MessageStatusUpdaterConfig config)
		{
			_messageModule = messageModule;
			_messageModule.MessageCreated += OnMessageCreated;
			_config = config;
			_scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
			_dataContext = context.CreateDbContext(new string[] { });
		}

		public void Dispose()
		{
			_messageModule.MessageCreated -= OnMessageCreated;

			_dataContext?.Dispose();
			_messageModule?.Dispose();
		}

		public void StartService()
		{
			var messages = _dataContext.Messages.ToList();
			foreach (var message in messages)
			{
				CreateJob(message);
			}

			_scheduler.Start();
		}

		private void OnMessageCreated(object sender, Message message)
		{
			CreateJob(message);
		}

		private void CreateJob(Message message)
		{
			var id = Guid.NewGuid().ToString();

			IJobDetail job = JobBuilder.Create<ChangeMessageDeliveryStatusJob>()
				.WithIdentity(id)
				.UsingJobData(new JobDataMap()
				{
					{"message", message},
					{"scheduler", _scheduler },
					{"dataContext", _dataContext },
					{"jobId", id }
				})
				.Build();

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity(id, "group")
				.WithSimpleSchedule(builder => builder.WithIntervalInMinutes(_config.UpdateIntervalMinutes).RepeatForever())
				.Build();

			_scheduler.ScheduleJob(job, trigger);
		}

		public class ChangeMessageDeliveryStatusJob : IJob
		{
			public async Task Execute(IJobExecutionContext context)
			{
				JobDataMap data = context.JobDetail.JobDataMap;

				Message message = (Message)data.Get("message");
				IScheduler scheduler = (IScheduler)data.Get("scheduler");
				DataContext dataContext = (DataContext)data.Get("dataContext");
				string jobId = (string)data.Get("jobId");

				if (message.Status == DeliveryStatus.Delivered)
				{
					await scheduler.DeleteJob(new JobKey(jobId));
					return;
				}

				message.StatusDescription = GetRandomStatus();
				if (message.StatusDescription == "Доставлено")
				{
					message.Status = DeliveryStatus.Delivered;
					await scheduler.DeleteJob(new JobKey(jobId));
				}

				dataContext.Messages.Update(message);
				await dataContext.SaveChangesAsync();
			}

			private string GetRandomStatus()
			{
				List<string> randomDescriptions = new List<string>()
				{
					"В Екатеринбурге, на распределительном пункте",
					"Проходит таможню",
					"На теплоходе, плывущем в Австралию",
					"Текущее местоположение не известно. Возможно мы его потеряли, но даже в этом мы не уверены. Мы делаем все возможное для исправления этой ситуации. Спасибо что остаетесь с нами, немотря на отвратительное качество услуг.",
					"Уважаемый пользователь вынуждены сообщить вам что в связи с недавним сбоем в наших серверах отключение света в серверной по независящим от нас причинам мы возвращаем все письма на первый пункт сортировки поэтому время доставки будет слегка увеличено у нас так не хватает времени что мы не можем даже раставить знаки препинания в данном предложение а если бы и было то мы бы не стали.",
					"В Сингапуре",
					"Едет в Тулу на перекладных",
					"Доставлено"
				};

				Random rnd = new Random();
				int randomNum = rnd.Next(0, randomDescriptions.Count);
				return randomDescriptions[randomNum];
			}
		}
	}
}
