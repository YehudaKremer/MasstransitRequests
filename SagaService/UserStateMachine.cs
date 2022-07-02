using MassTransit;
using ClassLibrary1.Commands;
using ClassLibrary1.Events;
using SagaService.Activities;
using MassTransit.SagaStateMachine;

namespace SagaService
{
    public class UserState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Uri ResponseAddress { get; set; }
        public int MyEventStatus { get; set; }
    }

    public class UserStateMachine : MassTransitStateMachine<UserState>
    {
        // Custom statuses for this saga
        public State GettingUserDetails { get; private set; }

        // Events/Commands used in this saga
        public Event<GetUserDetails> GetUserDetails { get; private set; }
        public Event<UserDetailsFound> UserDetailsFound { get; private set; }
        public Event MyEvent { get; private set; }

        public UserStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Initially(
                When(GetUserDetails)
                    .Then(c => c.Saga.ResponseAddress = c.ResponseAddress)
                    .Publish(c => new FindUserDetailsByID(c.Saga.CorrelationId, c.Message.ID))
                    .TransitionTo(GettingUserDetails));

            During(GettingUserDetails,
                When(UserDetailsFound)
                .Activity(x => x.OfType<ResponeUserDetailsActivity>()));

            CompositeEvent(MyEvent, x => x.MyEventStatus, GetUserDetails, UserDetailsFound);

            DuringAny(
                When(MyEvent)
                    .Then(context => Console.WriteLine("Got MyEvent, Current status is {0}", context.Saga.CurrentState)));

            SetCompletedWhenFinalized();
        }
    }


}
