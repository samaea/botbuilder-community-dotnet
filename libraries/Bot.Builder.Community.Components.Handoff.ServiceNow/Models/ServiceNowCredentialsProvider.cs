using Microsoft.Extensions.Configuration;

namespace Bot.Builder.Community.Components.Handoff.ServiceNow.Models
{
    public class ServiceNowCredentialsProvider : IServiceNowCredentialsProvider
    {
        public ServiceNowCredentialsProvider(IConfiguration configuration)
        {
            ServiceNowTenant = configuration["ServiceNowTenant"];
            UserName = configuration["ServiceNowUserName"];
            Password = configuration["ServiceNowPassword"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            //if (string.IsNullOrEmpty(MsAppId))
            //{
            //    MsAppId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            //}
        }

        public string ServiceNowTenant{ get; }

        public string UserName { get; }

        public string Password { get; }

        public string MsAppId { get; }
    }
}
