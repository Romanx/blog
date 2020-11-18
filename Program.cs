using Statiq.App;
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
            .RunAsync();
    }
}
