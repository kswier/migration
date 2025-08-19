using System;
using System.Configuration;
using System.Web.SessionState;

namespace Library
{
    public class ConfigObj
    {

        private static ConfigObj glblConfig = null;

        public static ConfigObj globalConfig()
        {
            if (glblConfig == null) 
                glblConfig = new ConfigObj();
            return glblConfig;
        }

        private String serverName
        {
            get
            {
                return ConfigurationManager.AppSettings["dbServerName"];
            }
        }

        private String databaseName
        {
            get
            {
                return ConfigurationManager.AppSettings["dbDatabaseName"];
            }
        }

        private String dbUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["dbUserName"];
            }
        }

        private String dbPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["dbPassword"];
            }
        }

        private bool integratedSecurity
        {
            get 
            {
                String rslt = ConfigurationManager.AppSettings["dbIntegratedSecurity"];
                return ((rslt != null) && (rslt.ToUpper() == "TRUE"));
            }
        }

        public String smtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["smtpServer"];
            }
        }

        public String smtpLogin
        {
            get
            {
                return ConfigurationManager.AppSettings["smtpLogin"];
            }
        }

        public String smtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["smtpPassword"];
            }
        }

        public String sendGridAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["sendGridAPI"];
            }
        }

        public String azureCommConnStr
        {
            get
            {
                return ConfigurationManager.AppSettings["azureCommConnStr"];
            }
        }



        public String supportEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["supportEmail"];
            }
        }
        public String senderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["senderEmail"];
            }
        }


        public ADODB getNewDB()
        {
            ADODB rslt = new ADODB();
            rslt.serverName = serverName;
            rslt.databaseName = databaseName;
            rslt.userName = dbUserName;
            rslt.password = dbPassword;
            rslt.applicationName = "aspnet";
            rslt.integratedSecurity = integratedSecurity;
            rslt.connected = true;
            return rslt;
        }

         
    }
}
