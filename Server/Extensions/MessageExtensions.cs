using Server.Model;
using Shared.Model;

namespace Server.Extensions;

public static class MessageExtensions
{
    public static MessageInfo ToMessageInfo(this Message m)
    {
        return new MessageInfo
        {
            Sender = m.Sender.ToUserInfo(),
            Text = m.Text,
            CreatedAt = m.CreatedAt
        };
    }
}