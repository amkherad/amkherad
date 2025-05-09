namespace RemoteProject.Shared.Abstractions.Messaging;

public interface IUserMessage : IMessageBase
{
    string UserId { get; set; }
}
