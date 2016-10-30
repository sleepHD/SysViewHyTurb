using System;
using System.Collections.Generic;
using System.Text;

namespace SysViewHyTurb.data
{
    using System.Threading;
    using System.Xml.Linq;
    using app.TcpHttpClient;
    using SysViewHyTurb.driver;
    using app.WebView;

    public class DataRepo
    {
        public delegate void ValueChangedHandler(KeyValuePair<string, object>[] keyValues);

        public event ValueChangedHandler ValueChanged;

        public int AppNum { get; set; }

        public object ReadValue(string name)
        {
            string varName, varProperty;
            if (name.Contains("."))
            {
                varName = name;
                varProperty = "Value";
            }
            else
            {
                var parts = name.Split(new char[] {'.'}, 2);
                varName = parts[0];
                varProperty = parts[1];
            }
            var variable = this.variables[varName];
            if (variable == null) return null;
            switch (varProperty)
            {
                case "Value":
                    return variable.Value;
                case "MaxValue":
                    return variable.MaxValue;
                case "MinValue":
                    return variable.MinValue;
                case "MaxRawValue":
                    return variable.MaxRawValue;
                case "MinRawValue":
                    return variable.MinRawValue;
                case "Convert":
                    return variable.Convert;
                default:
                    return null;
            }
        }

        public int WriteValue(string name, object value)
        {
            string varName, varProperty;
            if (name.Contains("."))
            {
                varName = name;
                varProperty = "Value";
            }
            else
            {
                var parts = name.Split(new char[] { '.' }, 2);
                varName = parts[0];
                varProperty = parts[1];
            }
            var variable = this.variables[varName];
            if (variable == null) return 0;
            switch (varProperty)
            {
                case "Value":
                    {
                        variable.Value = value;
                        return 1;
                    }
                case "MaxValue":
                    {
                        variable.MaxValue = (double)value;
                        return 1;
                    }
                case "MinValue":
                    {
                        variable.MinValue = (double)value;
                        return 1;
                    }
                case "MaxRawValue":
                    {
                        variable.MaxRawValue = (double)value;
                        return 1;
                    }
                case "MinRawValue":
                    {
                        variable.MinRawValue = (double)value;
                        return 1;
                    }
                case "Convert":
                    {
                        variable.Convert = (bool)value;
                        return 1;
                    }
                default:
                    return 0;
            }
        }

        public DataRepo(XDocument doc)
        {
            this.timers = new List<Timer>();

            var channelsElement = doc.Element("Configuration").Elements("Channels");
            foreach (var channelElement in channelsElement.Elements("Channel"))
            {
                var channel = new ModbusDriver(channelElement);
                this.channels.Add(channel);
                this.timers.Add(new Timer(this.PollChannel, this.channels.IndexOf(channel), Timeout.Infinite, 1000));

                foreach (var deviceElement in channelElement.Elements("Device"))
                {
                    foreach (var varElement in deviceElement.Elements("PV"))
                    {
                        var processVariable = new ProcessVariable(varElement);
                        this.variables[processVariable.Name] = processVariable;
                    }
                }
            }

            var httpUploadElement = doc.Element("Configuration").Element("HttpUploadApp");
            var httpUploadApp = new HttpUploadApp(httpUploadElement, this);


            var webViewElement = doc.Element("Configuration").Element("WebViewApp");
            var webViewApp = new WebViewApp(webViewElement, this);

            this.AppNum = 1;
        }

        private void PollChannel(Object stateInfo)
        {
            var index = (int)stateInfo;
            this.timers[index].Change(Timeout.Infinite, 1000);
            var channel = this.channels[index];
            foreach (var slave in channel.ModbusSlaves)
            {
                foreach (var grp in slave.VarGroups)
                {
                    channel.ProcessRegisterGroup(slave, grp);
                    var keyValues = new List<KeyValuePair<string, object>>();
                    if (grp.CommStatus == 0)
                    {
                        foreach (var regVar in grp.RegVars)
                        {
                            var variable = this.variables[regVar.Name];
                            variable.RawValue = regVar;
                            keyValues.Add(new KeyValuePair<string, object>(variable.Name, variable.Value));
                        }
                    }
                    if (ValueChanged != null)
                    {
                        ValueChanged(keyValues.ToArray());
                    }
                }
            }
            this.timers[index].Change(1000,  1000);
        }

        /// <summary>
        /// Dictionary of process variables
        /// </summary>
        private readonly Dictionary<string, ProcessVariable> variables = new Dictionary<string, ProcessVariable>();

        /// <summary>
        /// Underlying communication channels
        /// </summary>
        private readonly List<ModbusDriver> channels = new List<ModbusDriver>();

        private List<Timer> timers;
    }
}
