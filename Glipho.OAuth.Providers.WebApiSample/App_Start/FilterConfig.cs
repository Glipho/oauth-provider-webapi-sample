﻿using System.Web;
using System.Web.Mvc;

namespace Glipho.OAuth.Providers.WebApiSample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}