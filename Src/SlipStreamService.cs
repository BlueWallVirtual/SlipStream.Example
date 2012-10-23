
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
using System.Text;
using OpenSim.Services.Base;
using System.Reflection;
using Nini.Config;
using log4net;


namespace BlueWall.SlipStream.Example
{

	public class ExampleService:  ExampleServiceBase, IService
	{
		private static readonly ILog m_log =
            LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

		int Count
		{
			get; set;
		}

		string Message
		{
			get; set;
		}

		IConfig Config
		{
			get; set;
		}

		public ExampleService(IConfigSource config, string configName) 
			: base(config)
		{
			m_log.InfoFormat("[SLIPSTREAM]: running ExampleService constructor");
			Config = config.Configs[configName];
			if(Config == null)
			{
				m_log.InfoFormat("[SLIPSTREAM]: Configuration section {0} not found!", configName);
				return;
			}
			Message = Config.GetString("PageMessage", "Hello!");
		}

		public byte[] ServeMessage()
		{
			Count++;
			m_log.InfoFormat("[SLIPSTREAM]: Handler touched {0} times", Count.ToString());
			return UnicodeEncoding.ASCII.GetBytes(String.Format ("<h1>{0}</h1 <br>{1} Served!</h1>", Message, Count));
		}
	}  
}
