using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Infrastructures.Data.Entities;

public static class DapperConfig
{
    public static void AddDapper(this IServiceCollection services)
    {
        services.AddSingleton<IDapperContext, DapperContext>();
    }
}
