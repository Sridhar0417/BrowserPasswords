using BrowserPasswordHacking.Helpers;
using System;
using System.Data;

namespace BrowserPasswordHacking
{
    public partial class DisplayPasswords : System.Web.UI.Page
    {
        ChromePasswords objChromePasswords = new ChromePasswords();
        WifiPasswords objWifiPasswords = new WifiPasswords();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void btnReadPasswords_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtResults = null;
                switch (ddlPasswordsType.SelectedValue)
                {
                    case "1":
                        dtResults = objChromePasswords.Passwords();
                        break;
                    case "2":
                        dtResults = objWifiPasswords.GetWifiPasswords();
                        break;
                }
                if (dtResults != null && dtResults.Rows.Count > 0)
                {
                    gdResults.DataSource = dtResults;
                    gdResults.DataBind();
                }
            }
            catch { }
        }
    }
}