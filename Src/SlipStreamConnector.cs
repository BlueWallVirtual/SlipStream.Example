//
//  Copyright 2012  BlueWall Information Technologies, LLC
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.IO;
using OpenSim.Services.Base;
using OpenSim.Server.Base;
using System.Reflection;
using log4net;
using Nini.Config;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Server;
using OpenSim.Server.Handlers.Base;
using Mono.Addins;
using BlueWall.SlipStream.Example;


[assembly: Addin("SlipStreamExample", "0.1", Url="http://bluewallvirtual.com", Category="RobustPlugin")]
[assembly: AddinDependency("Robust", "0.1")]
[assembly: AddinDescription ("An example plugin for Robust")]
[assembly: AddinAuthor ("BlueWall Information Technologies, LLC")]
[assembly: AddinAuthor ("James Hughes")]
[assembly: AssemblyCopyright("Copyright (C) 2012 BlueWall Information Technologies, LLC (http://bluewallvirtual.com)")]


namespace BlueWall.SlipStream.Example
{
	[Extension(Path="/Robust/Connector")]
    public class ExampleConnector: ServiceConnector, IRobustConnector
    {
        static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region ServiceConnector property overrides
        /// <summary>
        /// Gets the URL where our bootstrap config may be found.
        /// </summary>
        /// <value>
        /// The config URL.
        /// </value>
		public override string ConfigURL
		{
            get { return "http://bluewallvirtual.com/configs/SlipStream.Example.xml"; }
		}

		/// <summary>
		/// Gets the name of the config section.
		/// </summary>
		/// <value>
		/// The name of the config section in our ini.
		/// </value>
		public override string ConfigName
		{
			get { return "SlipStream"; }
		}

		/// <summary>
		/// Gets our modular configuration file. This works on regular Robust connectors too.
		/// </summary>
		/// <value>
		/// The config file.
		/// </value>
		public override string ConfigFile
		{
			get; protected set;
		}

        /// <summary>
        /// Gets or sets the path to our plugin assembly so we are able to make
        /// a self contained module that includes the connector, service and handlers
        /// in one library.
        /// </summary>
        /// <value>
        /// The plugin path, will contain the assembly name if loaded from
        /// our ./bin directory.
        /// </value>
        public string PluginPath
        {
            get;
            set;
        }

		// Our LocalModule Name in our [Section]
		public string ServiceModuleName
		{
			get; private set;
		}

		// Our Local Module
		public IService ServiceModule
		{
			get; private set;
		}

		/// <summary>
		/// Gets the HTTP server.
		/// </summary>
		/// <value>
		/// The HTTP server.
		/// </value>
		public IHttpServer Server
		{
			get; private set;
		}

		/// <summary>
		/// Gets or sets the main configuration source.
		/// </summary>
		/// <value>
		/// The main configuration source.
		/// </value>
		public override IConfigSource Config
		{
			get; protected set;
		}
		#endregion
		public bool Enabled
		{
			get; private set;
		}
     
        // This will get called when we are loading from the registry
        public ExampleConnector()
        {
            m_log.Info("[SLIPSTREAM]: Unparameterized constructor called");
        }
        
        // The constructor will get called when loading via ini-ServiceConnectors. We don't want to do this
        // with dynamic plugins, as they will attempt to load twice.
        public ExampleConnector(IConfigSource config, IHttpServer server, string configName):
            base ( config, server, configName)
        {
            m_log.Info("[SLIPSTREAM]: Parameterized constructor called");
            Server = server;

            // Send the application's IConfigSource to our Configure method
            // so this works regardles of how we are being run.
            Configure(config);
        }

        /// <summary>
        /// Configure the plugin. This is the equivilent of our parameterized constructor
        /// for regular Robust connectors. But, we just get our configuration and return
        /// the port we want to run our connector on back to the application. Then the 
        /// application will send our server to the Initialize method
        /// </summary>
        /// <param name='config'>
        /// IConfigSource - Main Config.
        /// </param>
		public uint Configure(IConfigSource config)
		{
			m_log.Info("[SLIPSTREAM]: Running Configuration");
			Config = config;

			IConfig startconfig = Config.Configs["Startup"];
			string configdirectory = startconfig.GetString("ConfigDirectory", ".");

			ConfigFile = Path.Combine(configdirectory, "SlipStream.ini");
			m_log.InfoFormat("[SLIPSTREAM]: Configuration {0}", ConfigFile);

			// Look in our main config first
			IConfig serverConfig = Config.Configs[ConfigName];
            if (serverConfig == null)
				// Look for individual config file
				serverConfig = GetConfig ();
			if (serverConfig == null)
				throw new Exception(String.Format("[SlipStream]: Cannot file configuration for {0}, not loaded!", ConfigName));

			uint serverPort = (uint) serverConfig.GetInt("ServerPort", 0);
            ServiceModuleName = serverConfig.GetString("LocalServiceModule",
                    String.Empty);

            // Just something to test the config 
			m_log.InfoFormat("[SLIPSTREAM]: CodeWord {0}", serverConfig.GetString ("CodeWord","Blaah!"));

            if (ServiceModuleName == String.Empty)
                throw new Exception("No LocalServiceModule in config file");

            // We want to ship these plugins disabled to allow the user to make
            // adjustments to the ini prior to running. The bootstrap ini will be downloaded
            // when the plugin is enabled the first time. It should have an Enable and it
            // should be set to false. The user can disable the module in the console, make
            // edits (including setting Enable to true) then re-enable the plugin. It will 
            // load and run w/o restarting their Robust.
			if (serverConfig.GetBoolean("Enabled", false) == false)
			{
				m_log.Info("[SLIPSTREAM]: Module Disabled");
				Enabled = false;
				return 0;
			}
			else
				Enabled = true;

            Config.Merge(serverConfig.ConfigSource);

			return (uint) serverPort;
		}

		// Continue from our constructor to initialize our module
        public void Initialize(IHttpServer server)
        {
			m_log.Info("[SLIPSTREAM]: Running Initialize");
			Server = server;
            string modulepath = String.Empty;

            m_log.InfoFormat("[SLIPSTREAM]: Module Loading. Path {0}/{1}", PluginPath, ServiceModuleName);
            if ( PluginPath.Contains(".dll"))
            {
                modulepath = PluginPath;
            }
            else
            {
                modulepath = String.Format("{0}/{1}", PluginPath, ServiceModuleName);
            }

            Object[] args = new Object[] { Config, ConfigName };
            ServiceModule = ServerUtils.LoadPlugin<IService>(modulepath, args);

            server.AddStreamHandler(new GetHandler(ServiceModule));
        }

        public void Unload()
        {
            m_log.Info("[SlipStream]: Unloading");
            Server.RemoveStreamHandler("GET","/slipstream");
            Config.Configs.Remove(Config.Configs[ConfigName]);
        }
    }  
}
