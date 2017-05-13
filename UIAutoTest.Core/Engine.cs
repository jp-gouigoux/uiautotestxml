using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Xml;

namespace UIAutoTest.Core
{
    public class Engine
    {
        private string _AppPath = string.Empty;
        private int _LaunchTimeout = 5000;
        private XmlNodeList _Actions = null;

        public Engine(XmlDocument DomTest)
        {
            XmlNode NodeScenario = DomTest.SelectSingleNode("scenario");
            XmlNode NodeApplication = NodeScenario.SelectSingleNode("application");
            _AppPath = NodeApplication.Attributes["path"].Value;
            int.TryParse(NodeApplication.SelectSingleNode("launchWaitTimeout").InnerText, out _LaunchTimeout);
            _Actions = NodeScenario.SelectNodes("actions/*");
        }

        public void Run(int Timeout)
        {
            Process AppProcess = Process.Start(_AppPath);
            Thread.Sleep(_LaunchTimeout);
            AutomationElement AppForm = AutomationElement.FromHandle(AppProcess.MainWindowHandle);

            foreach (XmlNode NodeAction in _Actions)
            {
                if (NodeAction.Name.Equals("formSwitch"))
                {
                    Application.DoEvents();
                    Thread.Sleep(_LaunchTimeout);
                    AppForm = AutomationElement.FromHandle(Process.GetProcessesByName(AppProcess.ProcessName)[0].MainWindowHandle);
                    continue;
                }

                string AutomationId = NodeAction.Attributes["target"].Value;
                AutomationElement TargetElement = AppForm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.AutomationIdProperty, AutomationId));
                if (TargetElement == null) throw new ApplicationException("Impossible de trouver le composant d'automationId " + AutomationId);

                switch (NodeAction.Name)
                {
                    case "clearText":

                        TargetElement.SetFocus();
                        Thread.Sleep(200);
                        SendKeys.SendWait("^{HOME}");
                        SendKeys.SendWait("^+{END}");
                        SendKeys.SendWait("{DEL}");
                        break;

                    case "setText":

                        string TextValue = NodeAction.Attributes["value"].Value;
                        TargetElement.SetFocus();
                        Thread.Sleep(200);
                        SendKeys.SendWait(TextValue);
                        break;

                    case "buttonClick":

                        InvokePattern buttonPattern = TargetElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        buttonPattern.Invoke();
                        int WaitForAction = 500;
                        int.TryParse(NodeAction.Attributes["waitForAction"].Value, out WaitForAction);
                        Thread.Sleep(WaitForAction);
                        break;

                    case "linkClick":

                        InvokePattern linkPattern = TargetElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        linkPattern.Invoke();
                        int WaitForAction2 = 500;
                        int.TryParse(NodeAction.Attributes["waitForAction"].Value, out WaitForAction2);
                        Thread.Sleep(WaitForAction2);
                        break;

                    case "checkText":

                        string ReferenceValue = NodeAction.Attributes["value"].Value;
                        ValuePattern valuePattern = TargetElement.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        string ActualValue = valuePattern.Current.Value;
                        if (string.Compare(ActualValue, ReferenceValue) != 0)
                            throw new ApplicationException(NodeAction.Attributes["errMsg"].Value);
                        break;

                    case "checkLabel":

                        if (string.Compare(TargetElement.Current.Name, NodeAction.Attributes["value"].Value) != 0)
                            throw new ApplicationException(NodeAction.Attributes["errMsg"].Value);
                        break;
                }
            }

            AppProcess.Kill();
        }
    }
}
