using System;

namespace ClassLibrary1.Commands
{
    public record GetUserDetails(Guid CorrelationId, int ID);
    public record FindUserDetailsByID(Guid CorrelationId, int ID);
}

namespace ClassLibrary1.Events
{
    public record UserDetailsFound(Guid CorrelationId, User User);
    public record GotUserDetails(User User);
}
