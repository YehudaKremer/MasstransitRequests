using MassTransit;
using ClassLibrary1.Events;

namespace SagaService.Activities
{
    public class ResponeUserDetailsActivity : IStateMachineActivity<UserState, UserDetailsFound>
    {
        private readonly ConsumeContext context;

        public ResponeUserDetailsActivity(ConsumeContext context)
        {
            this.context = context;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<UserState, UserDetailsFound> c, IBehavior<UserState, UserDetailsFound> next)
        {
            var endpoint = await c.GetSendEndpoint(c.Saga.ResponseAddress);
            await endpoint.Send(new UserDetailsFound(c.Saga.CorrelationId, c.Message.User));

            await next.Execute(c).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<UserState, UserDetailsFound, TException> context, IBehavior<UserState, UserDetailsFound> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope(nameof(UserDetailsFound));
        }
    }
}
