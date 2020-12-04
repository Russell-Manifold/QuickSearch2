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
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QuickSearch2
{
    public partial class Default : System.Web.UI.Page
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] ReadWriteScope = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        private string fileId="";
        private SqlCommand cmd = new SqlCommand();
        private SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+HttpContext.Current.Server.MapPath("App_Data\\MainData.mdf")+";Integrated Security=True;Connect Timeout=30");
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
                            try
                            {
                                client.DownloadFile("https://drive.google.com/uc?export=download&id=" + fileId, HttpContext.Current.Server.MapPath(fileId + ".csv"));
                                LoadNewID(fileId);
                                LoadCSVToDb(HttpContext.Current.Server.MapPath(fileId + ".csv"));
                            }
                            catch { }
                        }
                    }
                }
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
                GridViewCustomer.DataSource = null;
                GridViewCustomer.DataBind();
                string Field = DropDownList1.SelectedValue.ToString();
                string SearchVal = txfSearch.Text.ToString();
                con.Open();
                SqlDataAdapter adapter;
                string AllFields = "HomeAssistPanicSOS,MemberStatus,PolicyType,MemberNo,Policy_Inception_date,CancellationDate,Title,Name,Surname,IDNumber,PassportNumber,TelHome,TelWork,TelOther,MainEmailAddress,AltEmailAddress,ResidentialAddressComplexNo,ResidentialAddressComplexName,ResidentialAddressStreetNo,ResidentialAddressStreetName,ResidentialAddressSuburb,ResidentialAddressPOcode,ResidentialAddressProvince";
                if (Field == "TelHome")
                {                   
                      adapter = new SqlDataAdapter($"SELECT {AllFields} FROM tblCustomer WHERE TelHome LIKE '%{SearchVal}%'  OR TelWork LIKE '%{SearchVal}%' OR TelOther LIKE '%{SearchVal}%'", con);
				}
				if (Field == "Name")
				{
                      adapter = new SqlDataAdapter($"SELECT {AllFields} FROM tblCustomer WHERE Name LIKE '%{SearchVal}%'  OR Surname LIKE '%{SearchVal}%'", con);                 

                }
				else
				{
                     adapter = new SqlDataAdapter($"SELECT {AllFields} FROM tblCustomer WHERE [{Field}] LIKE '%{SearchVal}%'", con);
                }
                DataSet ds = new DataSet();
				try
				{
                    adapter.Fill(ds);
                    GridViewCustomer.AutoGenerateColumns = true;                  
                    GridViewCustomer.DataSource = ds.Tables[0];
                    GridViewCustomer.DataBind();
					foreach(GridViewRow dr in GridViewCustomer.Rows)
					{
						try
						{
                            DateTime dts1 = Convert.ToDateTime(dr.Cells[4].Text);
                            dr.Cells[4].Text = dts1.ToString("dd MMM yyyy");
                           
						}
						catch (Exception)
						{
						}
						try
						{
                            DateTime dts2 = Convert.ToDateTime(dr.Cells[5].Text);
                            dr.Cells[5].Text = dts2.ToString("dd MMM yyyy");
                        }
						catch (Exception)
						{
						}
					}
                    con.Close();
					if (ds.Tables[0].Rows.Count>0)
					{
                         SendLogData(ds.Tables[0].Rows.Count+"", Field, SearchVal, DateTime.Now.ToString("dd MMM yyyy hh:mm:ss"));
                    }
                }
				catch (Exception ex)
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
        private void SendLogData(string Rows,string fieldName,string searchVal,string DateTime)
		{
                UserCredential credential;
                using (var stream = new FileStream(HttpContext.Current.Server.MapPath("credentials.json"), FileMode.Open, FileAccess.Read))
                {
                    //string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        ReadWriteScope,
                        "user",
                        CancellationToken.None
                        /*new FileDataStore(credPath)*/).Result;
                }

                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            try
            {
                String spreadsheetId = QuickSearch2.Properties.Settings.Default.LogDataSpreadsheet;   
                String range = "Sheet1!A1:E1";
                ValueRange body = new ValueRange();
                body.MajorDimension = "ROWS";
                var list = new List<object>() { Rows, fieldName, searchVal, DateTime };
                body.Values = new List<IList<object>> { list };

                var result = service.Spreadsheets.Values.Append(body, spreadsheetId, range);
                result.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                result.Execute();
            }
            catch {
                Response.Write("<script>alert('Unable to connect to specified Google Sheet Log Data')</script>");
            }
        }
        private void GetFileId()
		{
            UserCredential credential;
            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("credentials.json"), FileMode.Open, FileAccess.Read))
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

            try
            {
                //String spreadsheetId = "1LNUuQ3oHysNUpiIMpXyy6UXSC45zW9j7Hk0gF_AUSdk";
                //String spreadsheetId = "1-nJwaGKNHdKFI_6aKoe5M_5StReg1Kr89yBmWwAd0lk";
                String spreadsheetId =  QuickSearch2.Properties.Settings.Default.DefaultSpreadsheetID;
                String range = "Sheet1!A1";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values != null && values.Count > 0)
                {
                    var mainID = values[0];
                    fileId = mainID[0].ToString();
                    //Response.Write("<script>alert('The id found was " + mainID[0] + "')</script>");
                }
                else
                {
                    Response.Write("<script>alert('Could not find ID')</script>");
                }
            }
            catch {
                Response.Write("<script>alert('Unable to connect to specified Google Sheet')</script>");
            }
        }
		protected void btnClear_Click(object sender, EventArgs e)
		{
            DropDownList1.SelectedIndex = 0;
            txfSearch.Text = "";
        }
	}
}

