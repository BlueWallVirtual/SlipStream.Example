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
using System.Reflection;
using System.IO;
using OpenSim.Services.Base;
using OpenSim.Framework.Servers.HttpServer;
using log4net;


namespace BlueWall.SlipStream.Example
{
    public class GetHandler : BaseStreamHandler
    {
        static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		IService Service
		{
			get; set;
		}

		int Count
		{
			get; set;
		}

        public string HandlerType
        {
            get { return "GET"; }
        }

        public string URL
        {
            get { return "/slipstream"; }
        }

        public GetHandler(IService service) :
                base("GET", "/slipstream")
        {
            Service = service;
			Count = 0;
			m_log.Info("[SLIPSTREAM]: Handler Loading");
        }

        public override string ContentType
        {
            get { return "text/html"; }
        }
        public override byte[] Handle(string path, Stream requestData,
                IOSHttpRequest httpRequest, IOSHttpResponse httpResponse)
        {
			return Service.ServeMessage();
		}
	}  
}
