<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayPasswords.aspx.cs" Inherits="BrowserPasswordHacking.DisplayPasswords" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>This is demo application</p>
            <br />

            <asp:DropDownList ID="ddlPasswordsType" runat="server" AutoPostBack="false">
                <asp:ListItem Value="1" Selected="True">Google Chrome</asp:ListItem>
                <asp:ListItem Value="2">Connected Wifi</asp:ListItem>
            </asp:DropDownList>

            <br />

            <asp:Button ID="btnReadPasswords" runat="server" Text="Read Passwords" OnClick="btnReadPasswords_Click" />
            <br />
            <asp:GridView ID="gdResults" runat="server" AutoGenerateColumns="true" EmptyDataText="No records found">

            </asp:GridView>
        </div>
    </form>
</body>
</html>
