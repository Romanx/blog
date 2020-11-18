using Statiq.App;
using Statiq.Common;
using Statiq.Web;
using System.Threading.Tasks;

namespace Blog
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
          await Bootstrapper
            .Factory
            .CreateWeb(args)
            .AddReadingTimeMeta()
            .DeployToNetlify(
              Config.FromSetting<string>(""),
              Config.FromSetting<string>(WebKeys.NetlifyAccessToken))
            .RunAsync();
    }
}
