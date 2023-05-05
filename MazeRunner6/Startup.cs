using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunner6
{
    public class Startup
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddOptions<Config>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind("MazeConfig", settings);
            });
        }
    }
}
