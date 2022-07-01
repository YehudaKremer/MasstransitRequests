using MassTransit;
using ClassLibrary1.Commands;
using ClassLibrary1.Events;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly IRequestClient<GetUserDetails> client;

        public Worker(IRequestClient<GetUserDetails> client)
        {
            this.client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var result = await client.GetResponse<GotUserDetails>(new GetUserDetails(NewId.NextGuid(), 1));
            }
            catch (Exception ex)
            {

                throw;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}