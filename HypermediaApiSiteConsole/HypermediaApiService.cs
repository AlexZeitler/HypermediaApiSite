﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Web.Http.SelfHost;

namespace HypermediaApiSiteConsole
{
    partial class HypermediaApiService : ServiceBase
    {

        private HttpSelfHostServer _Host; 
        public HypermediaApiService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var baseAddress = (args.Length == 0) ? "http://hypermediaapi.com" : args[0];

                Trace.WriteLine(String.Format("Service starting on address:  {0}", baseAddress));

                var config = HypermediaApiConfiguration.ConfigureSite(baseAddress);

                _Host = new HttpSelfHostServer(config);

                _Host.OpenAsync().Wait();

                Trace.WriteLine("Service started");
            } catch(Exception ex)
            {
                Trace.TraceError((ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                this.Stop();
            }

        }

        protected override void OnStop()
        {
            try
            {
                Trace.WriteLine("Service stopping");

                _Host.CloseAsync().Wait();
            } catch(Exception ex)
            {
                Trace.TraceError((ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
        }
    }

    [RunInstaller(true)]
    public partial class SiteInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller _ProcessInstaller;
        private ServiceInstaller _Installer;

        public SiteInstaller()
        {
           
            _ProcessInstaller = new ServiceProcessInstaller();
            _Installer = new ServiceInstaller();

            _ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            _ProcessInstaller.Password = null;
            _ProcessInstaller.Username = null;

            _Installer.ServiceName = "HypermediaAPI";

            Installers.AddRange(
                new Installer[] { _ProcessInstaller, _Installer });

        }
    }
}
