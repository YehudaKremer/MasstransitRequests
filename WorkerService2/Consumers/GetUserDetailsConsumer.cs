using MassTransit;
using ClassLibrary1.Commands;
using ClassLibrary1.Events;

namespace WorkerService2.Consumers
{
    public class GetUserDetailsConsumer : IConsumer<FindUserDetailsByID>
    {
        public async Task Consume(ConsumeContext<FindUserDetailsByID> context)
        {
            //await Task.Delay(2000);
            await context.Publish(
                new UserDetailsFound(context.Message.CorrelationId, new ClassLibrary1.User(10, "Yehuda")));
        }
    }
}
