<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>APP - Dashboard</title>
    <link href="css/bootstrap.css" rel="stylesheet" />
    <nav class="navbar navbar-default">
        <div class="container-fluid">
            <ul class="nav nav-pills nav-justified">
                <li>
                    <a class="navbar-link" href="Default.aspx">
                        <img alt="Brand" src="Images/Alissa Paige Photography Color.png" />
                    </a>
                </li>
                <li><a class="navbar-link" href="Clients.aspx">Clients</a></li>
                <li><a class="navbar-link" href="Billing.aspx">Billing</a></li>
                <li><a class="navbar-link" href="Milage.aspx">Milage</a></li>
            </ul>
        </div>
    </nav>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="container-fluid">
                <div class="jumbotron">
                    <asp:Label runat="server" ID="UpcomingEventsLabel" Font-Size="XX-Large"></asp:Label>
                </div>
            </div>

        </div>
    </form>
    <footer>
        <asp:Label runat="server" ID="CopyrightLabel"></asp:Label>
    </footer>
</body>
</html>
