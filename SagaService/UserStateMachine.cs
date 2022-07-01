using MassTransit;
using ClassLibrary1.Commands;
using ClassLibrary1.Events;

namespace SagaService
{
    public class UserState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Uri ResponseAddress { get; set; }
    }

    public class UserStateMachine : MassTransitStateMachine<UserState>
    {
        // Custom statuses for this saga
        public State GettingUserDetails { get; private set; }

        // Events/Commands used in this saga
        public Event<GetUserDetails> GetUserDetails { get; private set; }
        public Event<UserDetailsFound> GotUserDetails { get; private set; }

        public UserStateMachine()
        {
            InstanceState(x => x.CurrentState, GettingUserDetails);

            Initially(
                When(GetUserDetails)
                    .Then(c =>
                    {
                        c.Saga.ResponseAddress = c.ResponseAddress;
                    })
                    .Publish(c => new FindUserDetailsByID(c.Saga.CorrelationId, c.Message.ID))
                    .TransitionTo(GettingUserDetails));

            During(GettingUserDetails,
                When(GotUserDetails)
                .ThenAsync(async c =>
                {
                    var endpoint = await c.GetSendEndpoint(c.Saga.ResponseAddress);
                    await endpoint.Send(new UserDetailsFound(c.Saga.CorrelationId, c.Message.User));
                })
                .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}
