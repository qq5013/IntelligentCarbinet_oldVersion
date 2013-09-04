using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o;
using PublicConfig;
using InventoryMSystem;

namespace IntelligentCarbinet
{
    public class appConfig
    {
        public appConfig() { }
        public int port = 9001;
        public int interval = 10000;
        public emuTagInfoFormat dataFormat = emuTagInfoFormat.标准;
        static string configFilePath = staticClass.configFilePath;
        public static appConfig getDefaultConfig()
        {
            appConfig config = new appConfig();
            IObjectContainer db = Db4oFactory.OpenFile(appConfig.configFilePath);
            try
            {
                IList<appConfig> list = db.Query<appConfig>(typeof(appConfig));

                if (list.Count > 0)
                {
                    config = list[0];
                }

            }
            finally
            {
                db.Close();
            }
            return config;
        }

        public static void saveConfig(appConfig config)
        {
            IObjectContainer db = Db4oFactory.OpenFile(appConfig.configFilePath);
            try
            {
                IList<appConfig> list = db.Query<appConfig>(typeof(appConfig));
                if (list.Count <= 0)
                {
                    db.Store(config);
                }
                else
                {
                    appConfig.copy(config, list[0]);
                    db.Store(list[0]);
                }

            }
            finally
            {
                db.Close();
            }
        }
        public static void copy(appConfig source, appConfig dest)
        {
            dest.port = source.port;
            dest.interval = source.interval;
        }

    }

}
