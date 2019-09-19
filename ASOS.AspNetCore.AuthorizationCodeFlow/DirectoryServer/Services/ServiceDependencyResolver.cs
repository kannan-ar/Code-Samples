﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServer.Services
{
    public static class ServiceDependencyResolver
    {
        public static void Resolve(IServiceCollection services)
        {
            services.AddTransient<IMongoService, MongoService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}
