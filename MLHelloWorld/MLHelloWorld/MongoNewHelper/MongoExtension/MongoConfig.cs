

using Microsoft.Extensions.Configuration;
using MZ.MongoProvider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace MZ.MongoProvider
{
    public class MongoConfig
    {
        private static MongoConfig _configuration;
        private readonly IConfigurationRoot _config;

        public MongoConfig(IConfigurationRoot Configuration)
        {
            _config = Configuration;
            _configuration = this;
        }

        public static MongoConfig DefaultInstance
        {
            get
            {
                return _configuration;
            }
        }
        public string MongConnectionString
        {
            get
            {
                return _config["MongConnectionString"];
            }
        }

    }

}
