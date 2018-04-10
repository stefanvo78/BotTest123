using Autofac;
using System.Web.Http;
using System.Configuration;
using System.Reflection;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;

namespace SimpleEchoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var docDbServiceEndpoint = new Uri(ConfigurationManager.AppSettings["DocumentDbUrl"]);
            var docDbEmulatorKey = ConfigurationManager.AppSettings["DocumentDbKey"];

            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                var store = new DocumentDbBotDataStore(docDbServiceEndpoint, docDbEmulatorKey);
                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
