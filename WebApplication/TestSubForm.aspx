<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestSubForm.aspx.cs" Inherits="WebApplication.TestSubForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server">
</head>
<body>
  <form id="ThisForm" runat="server">
    <h1>Test Sub Form</h1>
    <a href="WxeHandler.ashx?WxeFunctionType=WebApplication.TestRootFunction,WebApplication">Launch Test Page</a>
    <br/>
    <asp:LinkButton runat="server">Do Postback</asp:LinkButton>
    <br/>
    Note that the browser should remain on "Test Sub Form". With the Roslyn bug, the browser will return to the "Test Root Form" upon executing the postback.
  </form>
</body>
</html>
