<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestRootForm.aspx.cs" Inherits="WebApplication.TestRootForm" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server">
  <remotion:HtmlHeadContents runat="server"/>
</head>
<body>
  <form id="ThisForm" runat="server">
    <asp:ScriptManager runat="server"/>
<h1>Test Root Form</h1>
    <test:TestLinkButton runat="server" Text="Open Sub Form"/>
  </form>
</body>
</html>
