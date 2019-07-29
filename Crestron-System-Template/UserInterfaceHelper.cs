using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;
using Template.Interfaces;

namespace Template.UserInterface
{
    abstract class UserInterfaceComponent : ITrace
    {
        #region Properties
        public bool TraceEnabled { get; set; }
        public string TraceName { get; set; }
        #endregion

        #region Class variables
        protected ControlSystem cs;
        protected BasicTriList panel;
        protected uint[] digitalJoins = { 0 };
        protected uint[] analogJoins = { 0 };
        protected uint[] serialJoins = { 0 };
        protected uint[] smartObjectIDs = { 0 };
        #endregion

        #region Event handlers
        public void SigChangeHandler(BasicTriList device, SigEventArgs args)
        {
            try
            {
                switch (args.Sig.Type)
                {
                    case eSigType.Bool:
                        if (digitalJoins.Contains(args.Sig.Number))
                            DigitalJoinHandler(args.Sig.Number, args.Sig.BoolValue);
                        else
                            Trace("SigChangeHandler() undefined digital join.");
                        break;
                    case eSigType.UShort:
                        if (analogJoins.Contains(args.Sig.Number))
                            AnalogJoinHandler(args.Sig.Number, args.Sig.UShortValue);
                        else
                            Trace("SigChangeHandler() undefined analog join.");
                        break;
                    case eSigType.String:
                        if (serialJoins.Contains(args.Sig.Number))
                            SerialJoinHandler(args.Sig.Number, args.Sig.StringValue);
                        else
                            Trace("SigChangeHandler() undefined serial join.");
                        break;
                }
            }
            catch (Exception e)
            {
                Trace("SigChangeHandler() exception caught: " + e.Message);
            }
        }
        protected void Trace(string message)
        {
            if (TraceEnabled)
                CrestronConsole.PrintLine(String.Format("[{0}] {1}", TraceName, message.Trim()));
        }
        #endregion

        #region Abstract methods
        public abstract void Update();
        protected abstract void DigitalJoinHandler(uint join, bool value);
        protected abstract void AnalogJoinHandler(uint join, ushort value);
        protected abstract void SerialJoinHandler(uint join, string value);
        public abstract void SmartObjectHandler(GenericBase device, SmartObjectEventArgs args);
        #endregion
    }
}
