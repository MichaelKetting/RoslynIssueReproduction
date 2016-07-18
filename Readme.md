# Getting Started
* Set up WebApplication in an IIS running on Windows Server 2012R2 with .NET 4.5.x installed (not .NET 4.6 !) using a 64bit AppPool. 
It should also be possible to reproduce this on Windows Server 2008R2 and Widnows 7 / Windows 8 but this scenario wasn't tested.
* Use VS2015 (Update 3) to compile the solution.
* Launch WebApplication in the browser.
* Click "Launch Test Page"
* The browser now shows the "Test Root Form".
* Click "Open Sub Form"
* The browser now shows the "Test Sub Form".
* Click "Do Postback"
* The browser will return to "Test Root Form" if the bug is present and remain on "Test Sub Form" if the bug is not present.

# The Bug
In WebApplication, the [WebApplication/TestBaseFunction.cs] is missing the re-throw statement for the ThreadAbortException. 
If the re-throw is added or if the finally-block is removed, the bug will no longer show.
Note: The bug can also be observed when catching type Exception instead of simply ThreadAbortException.

# About the Code
Each request gets received by [WebLibrary/WxeHandler.cs] which implements IHttpHandler. There, the WxeFunction to execute is determined and either 
created for initial requests or loaded from the session state for subsequent requests. In the Exeucte method of the [WebLibrary/WxeFunction.cs], the
WxePageStep will get executed which in turn will perform a Server.Transfer to render the actual WebForms page. After the page is rendered, ASP.NET
will throw a ThreadAbortException which will then bubble out. 

When clicking "Open Sub Form" a sub function will execute by registering this new function on the internal stack during the page life cicle's postback event phase
and then using Server.Transfer to render the new page. When this page has been rendered, a ThreadAbortException is thrown and the stack will be unwound.

If the ThreadAbortException is falsly caught and swalled within the stack, the parent function's exeution logic will determine that the sub function 
has run to conlcusion and will remove it from the internal stack. This is where the bug causes the problem.

During the next postback, the parent function will then execute normally, showing the orginal page instead of showing the sub function's page.
