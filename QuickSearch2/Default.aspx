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
     
                <style type="text/css">
                        body
                        {
                            font-family: Arial;
                            font-size: 10pt;
                        }
                        td
                        {
                            cursor: pointer;
                        }
                        .hover_row
                        {
                            background-color: #ffd800;
                        }
            </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"/>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                    <div style="position: fixed; text-align: center; height: 100%; width: 100%; top: 0; right: 0; left: 0; z-index: 9999999; background-color: #000000; opacity: 0.5;">
                         <asp:Image ID="imgUpdateProgress" runat="server" ImageUrl="~/tenorwait.gif" AlternateText="Loading ..." ToolTip="Loading ..." style="padding: 10px;position:fixed;top:30%;left:40%; border-radius:1.5em" />
                   </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
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
                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" Height="2em"  /><br /><br />
                </div>
                </div>
                <div style="overflow:auto " >
                    <asp:GridView ID="GridViewCustomer" runat="server" CellPadding="3" ForeColor="Black" GridLines="Vertical"  AutoGenerateColumns="false" >
						<RowStyle />
                        <AlternatingRowStyle BackColor="WhiteSmoke" />
						<FooterStyle BackColor="#CCCCCC" />
						<HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                        <Columns>
                            <asp:TemplateField HeaderText="SOS" ItemStyle-Width="2em">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CheckBox1" runat="server" Enabled="false" Checked='<%# Eval("HomeAssistPanicSOS") %>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            <asp:BoundField DataField="MemberStatus" HeaderText="Status" />
                            <asp:BoundField DataField="PolicyType"  HeaderText="Policy Type"/>
                            <asp:BoundField DataField="MemberNo" HeaderText="Member No" />
                            <asp:BoundField DataField="Policy_Inception_date" HeaderText="Inception_Date"  ItemStyle-Width="7em"  ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Title" HeaderText="Title" />
                            <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="150px" />                           
                            <asp:BoundField DataField="Surname" HeaderText="Surname" ItemStyle-Width="150px" />                           
                            <asp:BoundField DataField="IDNumber" HeaderText="ID Number" />                           
                            <asp:BoundField DataField="TelHome" HeaderText="Tel Home" />                           
                            <asp:BoundField DataField="TelWork" HeaderText="Tel Work" />                           
                            <asp:BoundField DataField="TelOther" HeaderText="Tel Other" />                           
                            <asp:BoundField DataField="MainEmailAddress" HeaderText="Email Address" />                           
                            <asp:BoundField DataField="AltEmailAddress" HeaderText="Alt Email Address" />                           
                            <asp:BoundField DataField="ResidentialAddressComplexNo" HeaderText="Complex No" />                           
                            <asp:BoundField DataField="ResidentialAddressComplexName" HeaderText="Complex Name" ItemStyle-Width="150px" />                           
                            <asp:BoundField DataField="ResidentialAddressStreetNo" HeaderText="Street No" ItemStyle-HorizontalAlign="Center" />                           
                            <asp:BoundField DataField="ResidentialAddressStreetName" HeaderText="Street Name" ItemStyle-Width="150px" />                           
                            <asp:BoundField DataField="ResidentialAddressSuburb" HeaderText="Suburb" />                           
                            <asp:BoundField DataField="ResidentialAddressProvince" HeaderText="Code" />                                                    
                        </Columns>
					</asp:GridView>
                    </div>
                 <script type="text/javascript">
                     $(function () {
                         $("[id*=GridViewCustomer] td").hover(function () {
                             $("td", $(this).closest("tr")).addClass("hover_row");
                         }, function () {
                             $("td", $(this).closest("tr")).removeClass("hover_row");
                         });
                     });
        </script>
         </ContentTemplate>
        </asp:UpdatePanel>      
    </form>
</body>
</html>
