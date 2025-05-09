namespace RemoteProject.Shared.Abstractions.Messaging;

public interface IGroupMessage : IMessageBase
{
    string GroupId { get; set; }
}
