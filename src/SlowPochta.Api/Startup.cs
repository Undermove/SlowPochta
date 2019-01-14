using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SlowPochta.Api.Configuration;
using SlowPochta.Api.WebSocketBehaviors;
using SlowPochta.Business.Module;
using SlowPochta.Business.Module.Configuration;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Core;
using SlowPochta.Data.Repository;
using Swashbuckle.AspNetCore.Swagger;

namespace SlowPochta.Api
{
	public class Startup
	{
		private ServiceProvider _containerProvider;
		private WebSocketsModule _webSocketsModule;
		private readonly AuthOptions _authOptions;
		private readonly IConfigurationRoot _configuration;

		public Startup()
		{
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();
			_authOptions = new AuthOptions(_configuration);
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(_ => _configuration);
			services.AddSingleton(_ => _authOptions);
			services.AddSingleton(new LoggerFactory().AddConsole(_configuration.GetSection("Logging")));
			services.AddLogging();

			services.AddSingleton<DesignTimeDbContextFactory>();
			services.AddSingleton<AuthModule>();
			services.AddSingleton<UsersModule>();
			services.AddSingleton<WebSocketsModule>();
			services.AddSingleton<MessageStatusUpdater>();
			services.AddSingleton<MessageStatusUpdaterConfig>();

			services.AddTransient<IWebSocketBehavior, AuthConnectionBehavior>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.RequireHttpsMetadata = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						// укзывает, будет ли валидироваться издатель при валидации токена
						ValidateIssuer = true,
						// строка, представляющая издателя
						ValidIssuer = _authOptions.Issuer,

						// будет ли валидироваться потребитель токена
						ValidateAudience = true,
						// установка потребителя токена
						ValidAudience = _authOptions.Audience,
						// будет ли валидироваться время существования
						ValidateLifetime = true,

						// установка ключа безопасности
						IssuerSigningKey = _authOptions.SymmetricSecurityKey,
						// валидация ключа безопасности
						ValidateIssuerSigningKey = true,
					};
				});
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v0", new Info { Title = "SlowPochta API", Version = "v0.0.0" });
			});

			services.AddMvc();
			services.AddCors();

			_containerProvider = services.BuildServiceProvider();

			//Singleton may not be singleton fix. See this for details https://weblogs.thinktecture.com/pawel/2017/08/aspnet-core-beware-singleton-may-not-be-singleton.html
			MessageModule messageModule = new MessageModule(_containerProvider.GetService<DesignTimeDbContextFactory>());
			services.AddSingleton(messageModule);
			_containerProvider = services.BuildServiceProvider();

			_webSocketsModule = _containerProvider.GetService<WebSocketsModule>();

			Start();
		}

		public void Start()
		{
			var loggerFactory = _containerProvider.GetService<ILoggerFactory>();
			ApplicationLogging.LoggerFactory = loggerFactory;

			var msu = _containerProvider.GetService<MessageStatusUpdater>();
			msu.StartService();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseCors(builder =>
			{
				builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
			});

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v0/swagger.json", "SlowPochta API V0");
			});

			app.UseDefaultFiles();
			app.UseStaticFiles();

            var webSocketOptions = new WebSocketOptions() 
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);


			app.Use(async (context, next) =>
			{
				await _webSocketsModule.ProcessWebSocketRequest(context, next);
			});

			app.UseAuthentication();
			app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
