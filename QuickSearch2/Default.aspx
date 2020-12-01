<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuickSearch2.Default" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"/>
        <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
                <div>
                    Search By
                    <asp:DropDownList ID="DropDownList1" runat="server">
						<asp:ListItem>-Select-</asp:ListItem>
						<asp:ListItem Value="Name">Client Name</asp:ListItem>
						<asp:ListItem Value="IDNumber">ID Number</asp:ListItem>
						<asp:ListItem Value="MemberNo">Member Number</asp:ListItem>
						<asp:ListItem Value="PolicyType">Policy Name</asp:ListItem>
						<asp:ListItem Value="TelHome">Phone</asp:ListItem>
					</asp:DropDownList>
                    Search for 
                    <asp:TextBox ID="txfSearch" runat="server"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" />
                </div>
                <div>
                    <asp:GridView ID="GridViewCustomer" runat="server" Height="144px" Width="582px" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical">
						<AlternatingRowStyle BackColor="#CCCCCC" />
						<FooterStyle BackColor="#CCCCCC" />
						<HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
						<PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
						<SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
						<SortedAscendingCellStyle BackColor="#F1F1F1" />
						<SortedAscendingHeaderStyle BackColor="#808080" />
						<SortedDescendingCellStyle BackColor="#CAC9C9" />
						<SortedDescendingHeaderStyle BackColor="#383838" />
					</asp:GridView>
                    <%--<asp:GridView ID="GridViewCustomer" runat="server" DataSourceID="SqlDataSource1" Height="144px" Width="582px"></asp:GridView>
                	<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT TOP 3 * FROM [tblCustomer]"></asp:SqlDataSource>
               --%> </div>
           <%-- </ContentTemplate>
        </asp:UpdatePanel>--%>
    </form>
</body>
</html>
