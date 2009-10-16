﻿#region File Description
//-----------------------------------------------------------------------------
// SLNet.cs
//
// StiLib Network Service.
// Copyright (c) Zhang Li. 2009-04-30.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Experiment Service Contract
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IExServiceCallback))]
    public interface IExService
    {
        /// <summary>
        /// Invoke an Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [OperationContract]
        string Invoke(string ex);
        /// <summary>
        /// Invoke an Experiment Script 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        [OperationContract]
        string InvokeScript(string ex, string script);
        /// <summary>
        /// Get All Avalible Experiment Names
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string[] GetEx();
        /// <summary>
        /// Terminate an Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [OperationContract]
        string Terminate(string ex);
        /// <summary>
        /// Gets/Sets Experiment Run_Stop state
        /// </summary>
        bool RunStop
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
        /// <summary>
        /// Subscribe/Unsubscribe to service's callback client list
        /// </summary>
        /// <param name="sub_unsub"></param>
        /// <returns></returns>
        [OperationContract]
        string Subscribe(bool sub_unsub);
    }

    /// <summary>
    /// StiLib Experiment Service Callback Contract
    /// </summary>
    public interface IExServiceCallback
    {
        /// <summary>
        /// Client Operation when server Run_Stop state changed
        /// </summary>
        /// <param name="runstop"></param>
        /// <returns></returns>
        [OperationContract]
        string OnRunStop(bool runstop);
    }


    /// <summary>
    /// StiLib Experiment Service
    /// </summary>
    public class ExService : SLForm, IExService
    {
        /// <summary>
        /// Service Callback Client List
        /// </summary>
        public List<IExServiceCallback> CallbackClients=new List<IExServiceCallback>();


        /// <summary>
        /// Init to Default -- buffercount: 1, width: 800, height: 600, refreshrate: 0, vsync: false, showcursor: true, border: true, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        public ExService()
            : base(1, 800, 600, 0, false, true, true, false, Vector3.One)
        {
        }

        /// <summary>
        /// Init Using StiLib Configuration File
        /// </summary>
        /// <param name="configfile">empty/null to load default StiLib.dll.config file, otherwise indicate full file path</param>
        public ExService(string configfile)
            : base(configfile)
        {
        }

        /// <summary>
        /// Initializes with default -- buffercount: 1, border: true, sizable: false, gamma: (1.0, 1.0, 1.0)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public ExService(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
            : base(1, width, height, refreshrate, isvsync, isshowcursor, true, false, Vector3.One)
        {
        }

        /// <summary>
        /// Initializes the StiLib Experiment Service Form hosting XNA GraphicsDevice
        /// </summary>
        /// <param name="buffercount">0-3</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate">windowed mode(0), fullscreen mode(>0)</param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        /// <param name="isborder"></param>
        /// <param name="issizable"></param>
        /// <param name="gamma">current R, G, B gamma value</param>
        public ExService(int buffercount, int width, int height, int refreshrate, bool isvsync, bool isshowcursor, bool isborder, bool issizable, Vector3 gamma)
            : base(buffercount, width, height, refreshrate, isvsync, isshowcursor, isborder, issizable, gamma)
        {
        }


        /// <summary>
        /// Invoke an Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual string Invoke(string ex)
        {
            string ext = ex.Substring(ex.LastIndexOf(".") + 1);

            try
            {
                switch (ext)
                {
                    case "exe":
                        Process.Start(SLConfig["stilib"] + ex);
                        break;
                    case "fsx":
                        Process.Start(SLConfig["fsi"], SLConfig["stilib"] + ex);
                        break;
                    case "py":
                        Process.Start(SLConfig["ipy"], SLConfig["stilib"] + ex);
                        break;
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Invoke an Experiment Script
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public virtual string InvokeScript(string ex, string script)
        {
            StreamWriter writer = new StreamWriter(SLConfig["stilib"] + ex);
            writer.Write(script);
            writer.Flush();
            writer.Close();
            return Invoke(ex);
        }

        /// <summary>
        /// Get Experiment Names List
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetEx()
        {
            var fsx = Directory.GetFiles(SLConfig["stilib"], "*.fsx");
            var py = Directory.GetFiles(SLConfig["stilib"], "*.py");
            var exe = Directory.GetFiles(SLConfig["stilib"], "*.exe");

            var temp = fsx.Concat<string>(py).Concat<string>(exe);
            string[] ex = temp.ToArray<string>();
            for (int i = 0; i < ex.Length; i++)
            {
                ex[i] = Path.GetFileName(ex[i]);
            }
            return ex;
        }

        /// <summary>
        /// Terminate an Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public virtual string Terminate(string ex)
        {
            string ext = ex.Substring(ex.LastIndexOf(".") + 1);
            string exp = "";
            try
            {
                switch (ext)
                {
                    case "exe":
                        exp = ex.Remove(ex.LastIndexOf("."));
                        break;
                    case "fsx":
                        exp = "fsi";
                        break;
                    case "py":
                        exp = "ipy";
                        break;
                }
                var p = Process.GetProcessesByName(exp);
                foreach (var processe in p)
                {
                    processe.Kill();
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Gets/Sets Experiment Run_Stop state
        /// </summary>
        public virtual bool RunStop
        {
            get
            {
                return GO_OVER;
            }
            set
            {
                GO_OVER = value;
                if (GO_OVER)
                {
                    SetFlow();
                    MarkHead();
                }
            }
        }

        /// <summary>
        /// Subscribe/Unsubscribe to service's callback client list
        /// </summary>
        /// <param name="sub_unsub"></param>
        /// <returns></returns>
        public virtual string Subscribe(bool sub_unsub)
        {
            try
            {
                var client = OperationContext.Current.GetCallbackChannel<IExServiceCallback>();
                if (CallbackClients.Contains(client))
                {
                    if(!sub_unsub)
                        CallbackClients.RemoveAt(CallbackClients.IndexOf(client));
                }
                else
                {
                    if(sub_unsub)
                        CallbackClients.Add(client);
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Call all subscribed client back when server Run_Stop state changed
        /// </summary>
        /// <param name="runstop"></param>
        /// <returns></returns>
        public virtual string OnRunStop(bool runstop)
        {
            if (CallbackClients.Count > 0)
            {
                string hr = "";
                List<int> removelist = new List<int>();
                foreach (IExServiceCallback callback in CallbackClients)
                {
                    try
                    {
                        hr = hr + callback.OnRunStop(runstop) + " --- ";
                    }
                    catch (CommunicationException)
                    {
                        hr = hr + "Communication Error ! This Callback Client Is Removed." + " --- ";
                        removelist.Add(CallbackClients.IndexOf(callback));
                    }
                }
                if (removelist.Count > 0)
                {
                    foreach (int invalidcallback in removelist)
                    {
                        CallbackClients.RemoveAt(invalidcallback);
                    }
                }
                return hr.Remove(hr.Length - 5);
            }
            else
            {
                return "No Callback Client Subscribed !";
            }
        }

    }

}
