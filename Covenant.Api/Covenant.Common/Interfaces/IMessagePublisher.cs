namespace Covenant.Common.Interfaces;

public interface IMessagePublisher
{
    Task SendMessageAsync<T>(T message, string queueOrTopic, IReadOnlyDictionary<string, object> properties = null);
}
