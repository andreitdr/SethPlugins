using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CppWrapper.Objects;

namespace CppWrapper
{
    internal static class Convertor
    {
        public static ApplicationStruct ConvertToApplicationStruct(this DiscordBotCore.Application app)
        {
            return new ApplicationStruct
            {
                ServerId = app.ServerID,
                Prefix = app.DiscordBotClient.BotPrefix,
                Token = app.DiscordBotClient.BotToken
            };
        }
    }
}
