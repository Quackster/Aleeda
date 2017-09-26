using System;
using System.Data;
using System.IO;
using System.Text;

using Aleeda.Core;
using Aleeda.Storage;
using Aleeda.Configuration;
using Aleeda.Net.Connections;

using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;

namespace Aleeda
{
    /// <summary>
    /// A static class that holds references to all managers and registered instances.
    /// </summary>
    public static class AleedaEnvironment
    {
        #region Fields
        private static Logging mLog = new Logging();
        private static ConfigurationModule mConfig;
        private static Encoding mTextEncoding = Encoding.GetEncoding(28591); // Latin1

        private static DatabaseManager mDatabaseManager;
        private static IonTcpConnectionManager mTcconnectionManager;

        public static string keyName;
        public static string dbKey;

        private static HabboHotel.HabboHotel mHabboHotel;
        private static HabboHotel.Cache.GetCache mGetCache;
        private static Encoding DefaultEncoding;
        #endregion

        #region Properties
        public static ConfigurationModule Configuration
        {
            get { return mConfig; }
        }
        #endregion

        #region Methods
        public static Logging GetLog()
        {
            return mLog;
        }

        /// <summary>
        /// Initializes the Ion server environment.
        /// </summary>
        public static void Initialize()
        {
            mLog.MinimumLogImportancy = LogType.Debug;
            Console.WriteLine(" [**] --> Initializing Aleeda environment.");

            DefaultEncoding = Encoding.Default;

            try
            {
                // Try to initialize configuration
                try
                {
                    mConfig = ConfigurationModule.LoadFromFile("settings.ini");
                }
                catch (FileNotFoundException ex)
                {
                    mLog.WriteError("Failed to load configuration file, exception message was: " + ex.Message);
                    AleedaEnvironment.Destroy();
                    return;
                }

                // Initialize database and test a connection by getting & releasing it
                DatabaseServer pDatabaseServer = new DatabaseServer(
                    AleedaEnvironment.Configuration["db1.server.host"],
                    AleedaEnvironment.Configuration.TryParseUInt32("db1.server.port"),
                    AleedaEnvironment.Configuration["db1.server.uid"],
                    AleedaEnvironment.Configuration["db1.server.pwd"]);

                Database pDatabase = new Database(
                    AleedaEnvironment.Configuration["db1.name"],
                    AleedaEnvironment.Configuration.TryParseUInt32("db1.minpoolsize"),
                    AleedaEnvironment.Configuration.TryParseUInt32("db1.maxpoolsize"));

                mDatabaseManager = new DatabaseManager(pDatabaseServer, pDatabase);
                mDatabaseManager.SetClientAmount(2);
                mDatabaseManager.ReleaseClient(mDatabaseManager.GetClient().Handle);
                mDatabaseManager.StartMonitor();

                // Initialize TCP listener
                mTcconnectionManager = new IonTcpConnectionManager(
                    AleedaEnvironment.Configuration.TryParseInt32("net.tcp.port"),
                    AleedaEnvironment.Configuration.TryParseInt32("net.tcp.maxcon"));
                mTcconnectionManager.GetListener().Start();

                
                // Try to initialize Habbo Hotel

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    /*for (int i = 0; i < 10000000000; i++)
                    {
                        dbClient.ExecuteQuery("INSERT INTO private_rooms (name, rating, description, ownerid, status, tags, thumbnail, petsAllowed, category, model, wallpaper, floorpaper, landscape) VALUES ('Meep','0' ,'New room!', 'Quackie', '0', '', 'HHHH', '1', '0', 'model_a', '000', '000', '0.0')");
                    }*/
                }


                mHabboHotel = new Aleeda.HabboHotel.HabboHotel();
                mGetCache = new HabboHotel.Cache.GetCache();

                //HabboHotel.Cache.Privilege.BootUp();

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE users SET online = '0'");
                    dbClient.ExecuteQuery("UPDATE users SET flat = '0'");
                }

                Console.WriteLine(" [**] --> Initialized Aleeda environment.");

                GC.Collect();
            }
            catch (Exception ex) // Catch all other exceptions
            {
                mLog.WriteError("Unhandled exception occurred during initialization of Aleeda environment. Exception message: " + ex);
            }
        }
        /// <summary>
        /// Destroys the Ion server environment and exits the application.
        /// </summary>
        public static void Destroy()
        {
            Console.WriteLine("Destroying Ion environment.");

            // Destroy Habbo Hotel 8-) (and all inner modules etc)
            if (GetHabboHotel() != null)
            {
                GetHabboHotel().Destroy(); 
            }

            // Clear connections
            if (GetTcpConnections() != null)
            {
                Console.WriteLine("Destroying TCP connection manager.");
                GetTcpConnections().GetListener().Stop();
                GetTcpConnections().GetListener().Destroy();
                GetTcpConnections().DestroyManager();
            }

            // Stop database
            if (GetDatabase() != null)
            {
                Console.WriteLine("Destroying database " + GetDatabase().ToString());
                GetDatabase().StopMonitor();
                GetDatabase().DestroyClients();
                GetDatabase().DestroyManager();
            }
           
            Console.WriteLine("Press a key to exit.");

            Console.ReadKey();
            System.Environment.Exit(0);
        }

        /// <summary>C:\Users\Alex\Desktop\Projects\C#\Projects\AleedaEmulator\AleedaEnvironment.cs
        /// Returns the configuration module.
        /// </summary>
        /// <returns>ConfigurationModule holding configuration values for Ion.</returns>
        public static ConfigurationModule GetConfiguration()
        {
            return mConfig;
        }
        /// <summary>
        /// Returns the default System.Text.Encoding for encoding and decoding text.
        /// </summary>
        /// <returns>System.Text.Encoding</returns>
        public static Encoding GetDefaultTextEncoding()
        {
            return mTextEncoding;
        }

        public static DatabaseManager GetDatabase()
        {
            return mDatabaseManager;
        }
        public static IonTcpConnectionManager GetTcpConnections()
        {
            return mTcconnectionManager;
        }
        public static HabboHotel.HabboHotel GetHabboHotel()
        {
            return mHabboHotel;
        }
        public static HabboHotel.Cache.GetCache GetCache()
        {
            return mGetCache;
        }
        public static long DateTimeToUnixTimestamp(DateTime _DateTime)
        {
            TimeSpan _UnixTimeSpan = (_DateTime - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)_UnixTimeSpan.TotalSeconds;
        }
        public static string Timestamp()
        {
            return (string)DateTimeToUnixTimestamp(DateTime.Now).ToString();
        }
        public static String TimestampInt
        {
            get
            {
                return DateTimeToUnixTimestamp(DateTime.Now).ToString();
            }
        }
        public static int GenerateRandomNum(int min, int max)
        {
            //List<int> randomList = new List<int>();
            Random random = new Random();
            int RandomInteger = random.Next(min, max);

            /*while (randomList.Contains(RandomInteger))
            {*/
                //randomList.Add(RandomInteger);

                return RandomInteger;
            /*}
            return 0;*/
        }

        public static bool IsValidAlphaNumeric(string inputStr)
        {
            //Thanks to Uber.
            if (string.IsNullOrEmpty(inputStr))
            {
                return false;
            }

            for (int i = 0; i < inputStr.Length; i++)
            {
                if (!(char.IsLetter(inputStr[i])) && (!(char.IsNumber(inputStr[i]))))
                {
                    return false;
                }
            }

            return true;
        }
        internal static Encoding GetDefaultEncoding()
        {
            return DefaultEncoding;
        }

        #endregion
    }
}
