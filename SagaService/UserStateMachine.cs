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
        public State UserDetailsReturned { get; private set; }

        // Events/Commands used in this saga
        public Event<GetUserDetails> GetUserDetails { get; private set; }
        public Event<UserDetailsFound> UserDetailsFound { get; private set; }

        public UserStateMachine()
        {
            InstanceState(x => x.CurrentState, GettingUserDetails, UserDetailsReturned);

            Initially(
                When(GetUserDetails)
                    .Then(c =>
                    {
                        c.Saga.ResponseAddress = c.ResponseAddress;
                    })
                    .Publish(c => new FindUserDetailsByID(c.Saga.CorrelationId, c.Message.ID))
                    .TransitionTo(GettingUserDetails));

            During(GettingUserDetails,
                When(UserDetailsFound)
                .ThenAsync(async context =>
                {
                    var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress);
                    await endpoint.Send(new GotUserDetails(context.Message.User));
                })
                .TransitionTo(UserDetailsReturned)
                .Finalize());

            SetCompletedWhenFinalized();
        }
    }
}
