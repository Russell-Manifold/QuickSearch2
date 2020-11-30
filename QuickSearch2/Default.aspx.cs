using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuickSearch2
{
    public partial class Default : System.Web.UI.Page
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        private string fileId="";
        private SqlCommand cmd = new SqlCommand();
        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Visual Studios\repo\QuickSearch2\QuickSearch2\App_Data\MainData.mdf;Integrated Security=True;Connect Timeout=30");
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
                if (fileId == "")
                {
                    GetFileId();
                    cmd.Connection = con;
                    string currentID = GetCurrentLoadedID();
                    if (fileId != currentID)
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile("https://drive.google.com/uc?export=download&id=" + fileId, @"D:\Visual Studios\repo\QuickSearch2\QuickSearch2\" + fileId + ".csv");
                            LoadNewID(fileId);
                            LoadCSVToDb(@"D:\Visual Studios\repo\QuickSearch2\QuickSearch2\" + fileId + ".csv");
                        }
                    }
                }
            }                  
        }
        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DateTime dt = DateTime.Now;
                String formated = dt.ToString("dd.MM.yyyy");
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
			if (DropDownList1.SelectedIndex==0)
			{
                Response.Write("<script>alert('Please select a field')</script>");
            }
			else
			{
                string Field = DropDownList1.SelectedValue.ToString();
                string SearchVal = txfSearch.Text.ToString();
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM tblCustomer WHERE {Field} LIKE '%{SearchVal}%'", con);
                //SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM tblCustomer WHERE Name LIKE '%Ab%'", con);
                DataSet ds = new DataSet();
				try
				{
                    adapter.Fill(ds);
                    GridViewCustomer.AutoGenerateColumns = true;
                    GridViewCustomer.DataSource = ds.Tables[0];
                    GridViewCustomer.DataBind();
                    con.Close();
                }
				catch (Exception)
				{

				}               
            }          
        }
        private void LoadCSVToDb(string path)
		{
            string csvData = File.ReadAllText(path);
            csvData = csvData.Substring(csvData.IndexOf(Environment.NewLine) + 1);
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[24] { 
                new DataColumn("IDVal",typeof(int)),
                new DataColumn("MemberStatus",typeof(string)),
                new DataColumn("PolicyType",typeof(string)),
                new DataColumn("MemberNo",typeof(string)),
                new DataColumn("Policy_Inception_date",typeof(DateTime)),
                new DataColumn("CancellationDate",typeof(DateTime)),
                new DataColumn("Title",typeof(string)),
                new DataColumn("Name",typeof(string)),
                new DataColumn("Surname",typeof(string)),
                new DataColumn("IDNumber",typeof(string)),
                new DataColumn("PassportNumber",typeof(string)),
                new DataColumn("TelHome",typeof(string)),
                new DataColumn("TelWork",typeof(string)),
                new DataColumn("TelOther",typeof(string)),
                new DataColumn("MainEmailAddress",typeof(string)),
                new DataColumn("AltEmailAddress",typeof(string)),
                new DataColumn("ResidentialAddressComplexNo",typeof(string)),
                new DataColumn("ResidentialAddressComplexName",typeof(string)),
                new DataColumn("ResidentialAddressStreetNo",typeof(string)),
                new DataColumn("ResidentialAddressStreetName",typeof(string)),
                new DataColumn("ResidentialAddressSuburb",typeof(string)),
                new DataColumn("ResidentialAddressPOcode", typeof(string)),
                new DataColumn("ResidentialAddressProvince",typeof(string)),
                new DataColumn("HomeAssistPanicSOS",typeof(bool))});
            foreach (string row in csvData.Split(Environment.NewLine.ToCharArray()))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    dt.Rows.Add();
                    int i = 1;
                    foreach (string cell in row.Split('|'))
                    {
						try
						{
                            dt.Rows[dt.Rows.Count - 1][i] = cell;
                            i++;
                        }
						catch (Exception ex)
						{
							if (ex.Message.Contains("Expected type is Boolean"))
							{
								if (cell == "Y")
								{
                                    dt.Rows[dt.Rows.Count - 1][i] = true;
                                    i++;
                                }
								else
								{
                                    dt.Rows[dt.Rows.Count - 1][i] = false;
                                    i++;
                                }                              
                            }
						}                      
                    }
                }
            }           
            SqlBulkCopy bulk = new SqlBulkCopy(con);
            bulk.DestinationTableName = "dbo.tblCustomer";
            con.Open();
            bulk.WriteToServer(dt);
            con.Close();
        }
        private void LoadNewID(string NewId)
		{
            con.Open();
            cmd.CommandText = "DELETE FROM tblID";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO tblID(FirstID) VALUES ('" + NewId + "')";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        private string GetCurrentLoadedID()
		{
            con.Open();            
            cmd.CommandText = "Select FirstID FROM tblID";
            string currentID = cmd.ExecuteScalar().ToString();
            con.Close();
            return currentID;
        }
        private void GetFileId()
		{
            UserCredential credential;
            using (var stream = new FileStream(@"D:\Visual Studios\repo\QuickSearch2\QuickSearch2\credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath)).Result;
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            String spreadsheetId = "1-nJwaGKNHdKFI_6aKoe5M_5StReg1Kr89yBmWwAd0lk";
            String range = "Sheet1!A1";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                var mainID = values[0];
                fileId = mainID[0].ToString();
                Response.Write("<script>alert('The id found was " + mainID[0] + "')</script>");
            }
            else
            {
                Response.Write("<script>alert('Could not find ID')</script>");
            }
        }
		protected void btnClear_Click(object sender, EventArgs e)
		{
            DropDownList1.SelectedIndex = 0;
            txfSearch.Text = "";
        }
	}
}

