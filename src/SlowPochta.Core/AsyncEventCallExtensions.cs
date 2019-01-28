using System;
using System.Threading.Tasks;

namespace SlowPochta.Core
{
	public static class AsyncEventCallExtensions
	{
		public static async Task InvokeEventAsync<T>(this Func<object, T, Task> handler, T eventArgs)
		{
			if (handler == null)
			{
				return;
			}

			Delegate[] invocationList = handler.GetInvocationList();
			Task[] handlerTasks = new Task[invocationList.Length];

			try
			{
				for (int i = 0; i < invocationList.Length; i++)
				{
					handlerTasks[i] = ((Func<object, T, Task>)invocationList[i])(new object(), eventArgs);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}


			await Task.WhenAll(handlerTasks);
		}
	}
}
