
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Breeze.Net.Tests
{
    public abstract class BreezeTest
    {
        protected BreezeService service;

        private IConfiguration Configuration { get; set; }

        public BreezeTest()
        {
            // the type specified here is just so the secrets library can
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<BreezeTest>();

            Configuration = builder.Build();

            var _apikey = Configuration["BreezeApiKey"];
            var _subdomain = Configuration["BreezeSubdomain"];

            service = new BreezeService(_subdomain, _apikey);

            RunBeforeTestFixture().Wait();
        }

        internal virtual Task RunBeforeTestFixture() => Task.CompletedTask;
    }
}