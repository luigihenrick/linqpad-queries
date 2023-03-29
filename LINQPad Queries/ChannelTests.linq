<Query Kind="Program">
  <Namespace>System.Threading.Channels</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
</Query>

public readonly Channel<(string id, int count)> _queue = Channel.CreateUnbounded<(string id, int count)>(new UnboundedChannelOptions
{
	SingleReader = false,
	SingleWriter = false
});

async Task Main()
{
	var cts = new CancellationTokenSource();
	
	cts.CancelAfter(TimeSpan.FromSeconds(10));
	
	var loop1 = StartLoop(cts.Token);
	var loop2 = StartLoop(cts.Token);

	var tasks = Enumerable.Range(1, 10).Select(i => Task.Run(async () => 
	{
		await Task.Delay(1000); _queue.Writer.TryWrite((Guid.NewGuid().ToString(), i));
	}))
	.Union(new []{loop1, loop2});

	await Task.WhenAll(tasks);
}

public Task StartLoop(CancellationToken cts) => Task.Run(async () =>
{
	try
	{
		while (await _queue.Reader.WaitToReadAsync(cts))
		{
			while (_queue.Reader.TryRead(out (string Id, int Count) values))
			{
				await Handle(values.Id, values.Count);
			}
		}
	}
	catch (OperationCanceledException)
	{
		Console.WriteLine("SubscriptionConsumerCancelled");
	}
	catch (Exception ex)
	{
		Console.WriteLine("SubscriptionConsumerFailed");
	}
}, cts);

public async ValueTask Handle(string id, int count, [CallerMemberName] string caller = null)
{
	await Task.Delay(100);
	Console.WriteLine($"[{caller}] Id: {id}, count: {count}");
}