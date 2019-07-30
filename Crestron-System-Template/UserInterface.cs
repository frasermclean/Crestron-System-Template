using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.UI;
using Crestron.SimplSharpPro.DeviceSupport;
using Template.Interfaces;

namespace Template
{
    #region Supporting enums and classes
    public enum Page
    {
        Home,
        Settings,
    }
    public enum ModalPopup
    {
        None,
        Passcode,
    }
    class UserInterfaceState
    {
        public Page page = Page.Home;
        public ModalPopup modalPopup = ModalPopup.None;
    }
    enum DigitalJoin
    {
        SystemStartup = 10,
        SystemShutdown = 11,
    }
    enum AnalogJoin
    {
    }
    enum SerialJoin
    {
    }
    #endregion
    public class UserInterface : ITrace
    {
        #region Class variables
        Tsw760 panel;
        ControlSystem cs;
        UserInterfaceState state;
        #endregion

        #region Properties
        public bool TraceEnabled { get; set; }
        public string TraceName { get; set; }
        #endregion

        #region Constructor
        public UserInterface(ControlSystem cs, uint ipid, string projectName)
        {
            Initialize(cs, ipid, projectName, false);
        }
        public UserInterface(ControlSystem cs, uint ipid, string projectName, bool traceEnabled)
        {
            Initialize(cs, ipid, projectName, traceEnabled);
        }
        void Initialize(ControlSystem cs, uint ipid, string projectName, bool traceEnabled)
        {
            this.cs = cs;
            this.TraceEnabled = traceEnabled;
            this.TraceName = this.GetType().Name;

            // set defaults          
            state = new UserInterfaceState();
            state.page = Page.Home;
            state.modalPopup = ModalPopup.None;

            panel = new Tsw760(ipid, cs);

            // add callbacks           
            panel.OnlineStatusChange += new OnlineStatusChangeEventHandler(PanelOnlineStatusChangeHandler);
            panel.SigChange += new SigEventHandler(PanelSigChangeHandler);

            // register panel
            if (panel.Register() == eDeviceRegistrationUnRegistrationResponse.Success)
                Trace("Initialize() panel registered successfully.");
            else
            {
                Trace("Initialize() panel failed to register.");
                return;
            }

            // load SmartGraphics objects
            string sgdPath = String.Format("{0}\\{1}.sgd", Directory.GetApplicationDirectory(), projectName);
            if (File.Exists(sgdPath))
            {
                if (panel.LoadSmartObjects(sgdPath) > 0)
                {
                    Trace(String.Format("Initialize() loaded {0} SmartObjects.", panel.SmartObjects.Count));
                    foreach (KeyValuePair<uint, SmartObject> pair in panel.SmartObjects)
                        pair.Value.SigChange += new SmartObjectSigChangeEventHandler(PanelSmartObjectHandler);
                }
                else
                    Trace("Initialize() error, couldn't load any SmartObjects!");
            }
            else
            {
                Trace(String.Format("Initialize() error, SGD file: {0}, does not exist.", sgdPath));
            }
        }
        #endregion

        public void Update()
        {
        }

        void Trace(string message)
        {
            if (TraceEnabled)
                CrestronConsole.PrintLine(String.Format("[{0}] {1}", TraceName, message.Trim()));
        }

        #region Event handlers
        void PanelOnlineStatusChangeHandler(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            Trace("PanelOnlineStatusChangeHandler() panel online: " + args.DeviceOnLine);
            if (args.DeviceOnLine)
                Update();
        }
        void PanelSigChangeHandler(BasicTriList currentDevice, SigEventArgs args)
        {
            switch (args.Sig.Type)
            {
                case eSigType.Bool:
                    if (Enum.IsDefined(typeof(DigitalJoin), args.Sig.Number)) // check that signal is defined in enum
                    {
                        DigitalJoin join = (DigitalJoin)args.Sig.Number;
                        DigitalJoinHandler(join, args.Sig.BoolValue);
                    }
                    break;
                case eSigType.UShort:
                    break;
                case eSigType.String:
                    break;
                default:
                    Trace("PanelSigChangeHandler() unhandled signal type: " + args.Sig.Type);
                    break;
            }
        }
        void PanelSmartObjectHandler(GenericBase currentDevice, SmartObjectEventArgs args)
        {
            switch (args.SmartObjectArgs.ID)
            {
                default:
                    Trace("PanelSmartObjectHandler() unhandled SmartObject ID: " + args.SmartObjectArgs.ID);
                    break;
            }
        }
        #endregion

        #region Signal handlers
        void DigitalJoinHandler(DigitalJoin join, bool pressed)
        {
            if (pressed)
            {
                switch (join)
                {
                    case DigitalJoin.SystemStartup:
                        break;
                    case DigitalJoin.SystemShutdown:
                        break;
                    default:
                        Trace("DigitalJoinHandler() unhandled join: " + join);
                        break;
                }
            }
            else
            {
            }
        }
        #endregion
    }
}
