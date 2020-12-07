<%@ Page Title="QuickSearch" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QuickSearch2.Default" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trackbox Quicksearch</title>
     <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <meta name="description" content="" />
		<meta name="keywords" content="" />
     <link id="Link3" runat="server" rel="shortcut icon" href="~/search--v2.png" type="image/x-icon"/>
        <link id="Link4" runat="server" rel="icon" href="~/search--v2.png" type="image/ico" />		
    <noscript>
			<link rel="stylesheet" href="~/style.css" type="text/css" />
        </noscript>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"/>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
               <div style="width:80%">
                   <asp:Image ID="Image1" runat="server"  ImageUrl="~/trackboxmain.PNG" Width="350" style="margin-left:200px"/><br />
                <div style="width:100%;margin:auto; text-align:center">      
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
                    <asp:TextBox ID="txfSearch" runat="server" Width="100"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" Height="2em" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" Height="2em"  />
                </div>
                
                <div style="width:98%; margin:auto " >
                    <asp:GridView ID="GridViewCustomer" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="100%" >
						<AlternatingRowStyle BackColor="#CCCCCC" />
						<FooterStyle BackColor="#CCCCCC" />
						<HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
						<PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
						<SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
						<SortedAscendingCellStyle BackColor="#F1F1F1" />
						<SortedAscendingHeaderStyle BackColor="#808080" />
						<SortedDescendingCellStyle BackColor="#CAC9C9" />
						<SortedDescendingHeaderStyle BackColor="#383838" />
                        <Columns>
                            <asp:TemplateField HeaderText="SOS" ItemStyle-Width="3em">
                                                   <ItemTemplate>
                                                       <asp:CheckBox ID="CheckBox1" runat="server" Enabled="false" Checked='<%# Eval("HomeAssistPanicSOS") %>'/>
                                                   </ItemTemplate>
                                                </asp:TemplateField>
                            <asp:BoundField DataField="MemberStatus" HeaderText="Status" />
                            <asp:BoundField DataField="PolicyType"  HeaderText="Policy Type"/>
                            <asp:BoundField DataField="MemberNo" HeaderText="Member No" />
                            <asp:BoundField DataField="Policy_Inception_date" HeaderText="Policy Inception_date" HeaderStyle-Width="100" DataFormatString="{0:dd-MM-yyyy}" />
                            <%--<asp:BoundField DataField="CancellationDate" HeaderText="Cancellation Date" DataFormatString="{0:dd MM yyyy}"/>--%>
                            <asp:BoundField DataField="Title" HeaderText="Title" />
                            <asp:BoundField DataField="Name" HeaderText="Name" />                           
                            <asp:BoundField DataField="Surname" HeaderText="Surname" />                           
                            <asp:BoundField DataField="IDNumber" HeaderText="ID Number" />                           
                            <%--<asp:BoundField DataField="PassportNumber" HeaderText="Passport Number" />--%>                           
                            <asp:BoundField DataField="TelHome" HeaderText="TelHome" />                           
                            <asp:BoundField DataField="TelWork" HeaderText="TelWork" />                           
                            <asp:BoundField DataField="TelOther" HeaderText="TelOther" />                           
                            <asp:BoundField DataField="MainEmailAddress" HeaderText="Main Email Address" />                           
                            <asp:BoundField DataField="AltEmailAddress" HeaderText="Alt Email Address" />                           
                            <asp:BoundField DataField="ResidentialAddressComplexNo" HeaderText="Residential Address Complex No" />                           
                            <asp:BoundField DataField="ResidentialAddressComplexName" HeaderText="Address Complex Name" />                           
                            <asp:BoundField DataField="ResidentialAddressStreetNo" HeaderText="Address Street No" />                           
                            <asp:BoundField DataField="ResidentialAddressStreetName" ItemStyle-Width="100%" HeaderText="Address Street Name" />                           
                            <asp:BoundField DataField="ResidentialAddressSuburb" HeaderText="Suburb" />                           
                            <%--<asp:BoundField DataField="ResidentialAddressPOcode" HeaderText="Code" />--%>                           
                            <asp:BoundField DataField="ResidentialAddressProvince" HeaderText="Province" />                                                    
                        </Columns>
					</asp:GridView>
                    </div>
                    </div>
           </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
