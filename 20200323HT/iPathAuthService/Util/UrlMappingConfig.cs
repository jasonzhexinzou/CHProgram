using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace iPathAuthService.Util
{
    public class UrlMappingConfig
    {
        public static MappingCollection Setting = (ConfigurationManager.GetSection("urlMapping") as MappingConfiguration).Mapping;
    }
}