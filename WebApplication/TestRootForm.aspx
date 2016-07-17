<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestRootForm.aspx.cs" Inherits="WebApplication.TestRootForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml"> 
<head runat="server">
</head>
<body>
  <form id="ThisForm" runat="server">
    <h1>Test Root Form</h1>
    <a href="WxeHandler.ashx?WxeFunctionType=WebApplication.TestRootFunction,WebApplication">Launch Test Page</a>
    <br/>
    <test:TestLinkButton runat="server" Text="Open Sub Form"/>
  </form>
</body>
</html>
