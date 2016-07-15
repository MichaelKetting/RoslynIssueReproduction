<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestSubForm.aspx.cs" Inherits="WebApplication.TestSubForm" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server">
  <remotion:HtmlHeadContents runat="server"/>
</head>
<body>
  <form id="ThisForm" runat="server">
    <asp:ScriptManager runat="server"/>
    <h1>Test Sub Form</h1>
    <asp:LinkButton runat="server">Do Postback</asp:LinkButton>
    <br/>
    Note that the browser should remain on "Test Sub Form". With the Roslyn bug, the browser will return to the "Test Root Form" upon executing the postback.
  </form>
</body>
</html>
