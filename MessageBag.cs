namespace ShopBot
{
    public enum MessageType
    {
        ProductOrder,
        ProductRemoval,
        ProductDialogCancelled
    }

    public static class MessageBag
    {
        public static MessageBag<T> Of<T>(T content, MessageType type)
        {
            return new MessageBag<T>(content, type);
        }
    }

    public class MessageBag<T>
    {
        public T Content { get; }
        public MessageType Type { get; }

        public MessageBag(T content, MessageType type)
        {
            Content = content;
            Type = type;
        }
    }
}