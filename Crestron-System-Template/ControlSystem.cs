using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.Diagnostics;
using Crestron.SimplSharpPro.DeviceSupport;
using Template.Interfaces;

namespace Template
{
    public class ControlSystem : CrestronControlSystem, ITrace
    {
        #region System components
        #endregion

        #region Properties
        public bool TraceEnabled { get; set; }
        public string TraceName { get; set; }
        #endregion

        #region Initialization
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
            }
            catch (Exception ex)
            {
                string message = String.Format("ControlSystem() exception caught: {0}", ex.Message);
                Trace(message);
                ErrorLog.Exception(message, ex);
            }
        }
        public override void InitializeSystem()
        {
            try
            {
                // trace defaults
                this.TraceEnabled = true;
                this.TraceName = this.GetType().Name;
            }
            catch (Exception ex)
            {
                string message = String.Format("InitializeSystem() exception caught: {0}", ex.Message);
                Trace(message);
                ErrorLog.Exception(message, ex);
            }
        }
        #endregion

        #region Debugging
        private void Trace(string message)
        {
            if (TraceEnabled)
                CrestronConsole.PrintLine(String.Format("[{0}] {1}", TraceName, message.Trim()));
        }
        #endregion
    }
}
