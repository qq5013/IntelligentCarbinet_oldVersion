using System;
using System.Collections.Generic;
using System.Text;
using IntelligentCarbinet;

namespace PublicConfig
{
    public static class staticClass
    {
        public static DateTime timeBase = DateTime.Now;
        public static string PicturePath = @"商品图片\";
        public static string configFilePath = "app.config";
        public static DBType currentDbType = DBType.sqlite;
        public static string currentDBConnectString = string.Empty;

        /*
         Data Source=127.0.0.1\SQLExpress;Initial Catalog=IMS;User ID=sa;pwd=078515
         */
    }
}
