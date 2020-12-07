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
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2.Responses;
using QuickSearch2.Models;

namespace QuickSearch2
{
    public partial class Default : System.Web.UI.Page
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] ReadWriteScope = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "QuickSearchProject";
        static AccessLayer database=null;
        SettingsInfo mainSettings=null;
        //protected string googleplus_redirect_url = "https://localhost:44322/Default.aspx";
        //protected string googleplus_redirect_url = "http://localhost/QuickSearch";
        private string fileId = "";
        private void LoadJson()
		{
			if (mainSettings==null)
			{
                var s = File.ReadAllText(HttpContext.Current.Server.MapPath("credentials.json"));
                mainSettings = JsonConvert.DeserializeObject<WebInfo>(s).web;
            }          
		}
        private void LoadDb()
		{
			if (database==null)
			{
                database = new AccessLayer(HttpContext.Current.Server.MapPath("App_Data\\Sqlite.db3"));
			}
		}
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDb();
                LoadJson();
                GetAccessToken();
                if (fileId == "")
                {
                    GetFileId();                   
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
                            catch(Exception ex) { }
                        }
                    }
                }
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedIndex == 0)
            {
                Response.Write("<script>alert('Please select a field')</script>");
            }
            else
            {
                GridViewCustomer.DataSource = null;
                GridViewCustomer.DataBind();
                string Field = DropDownList1.SelectedValue.ToString();
                string SearchVal = txfSearch.Text.ToString();                              
                string AllFields = "HomeAssistPanicSOS,MemberStatus,PolicyType,MemberNo,Policy_Inception_date,CancellationDate,Title,Name,Surname,IDNumber,PassportNumber,TelHome,TelWork,TelOther,MainEmailAddress,AltEmailAddress,ResidentialAddressComplexNo,ResidentialAddressComplexName,ResidentialAddressStreetNo,ResidentialAddressStreetName,ResidentialAddressSuburb,ResidentialAddressPOcode,ResidentialAddressProvince";
                string adp= $"SELECT { AllFields} FROM Client WHERE[{ Field}] LIKE '%{SearchVal}%'";
                if (Field == "TelHome")
                {
                    adp=$"SELECT {AllFields} FROM Client WHERE TelHome LIKE '%{SearchVal}%'  OR TelWork LIKE '%{SearchVal}%' OR TelOther LIKE '%{SearchVal}%'";
                }
                if (Field == "Name")
                {
                    adp=$"SELECT {AllFields} FROM Client WHERE Name LIKE '%{SearchVal}%'  OR Surname LIKE '%{SearchVal}%'";

                }
                var output = database.FetchQuery(adp);
                try
                {                    
                    GridViewCustomer.AutoGenerateColumns = true;
                    GridViewCustomer.DataSource = output;
                    GridViewCustomer.DataBind();
                    foreach (GridViewRow dr in GridViewCustomer.Rows)
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
					if (output.Count > 0)
					{
						SendLogData(output.Count + "", Field, SearchVal, DateTime.Now.ToString("dd MMM yyyy hh:mm:ss"));
					}
				}
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Could not load due to " + ex + "')</script>");
                }
            }
        }
        private void LoadCSVToDb(string path)
        {
			string csvData = File.ReadAllText(path);
			csvData = csvData.Substring(csvData.IndexOf(Environment.NewLine) + 1);
            List<Client> AllClients = new List<Client>();
            string[] ros = new string[24];
            foreach (string row in csvData.Split(Environment.NewLine.ToCharArray()))
			{            
				if (!string.IsNullOrEmpty(row))
				{
                    ros = row.Split('|');
                    Client cl = new Client
                    {
                        MemberStatus=ros[0],
                        PolicyType = ros[1],
                        MemberNo = ros[2],
                        Policy_Inception_date = Convert.ToDateTime(ros[3]),
                        CancellationDate = Convert.ToDateTime(ros[4]),
                        Title = ros[5],
                        Name = ros[6],
                        Surname = ros[7],
                        IDNumber = ros[8],
                        PassportNumber = ros[9],
                        TelHome = ros[10],
                        TelWork = ros[11],
                        TelOther = ros[12],
                        MainEmailAddress = ros[13],
                        AltEmailAddress = ros[14],
                        ResidentialAddressComplexNo = ros[15],
                        ResidentialAddressComplexName = ros[16],
                        ResidentialAddressStreetNo = ros[17],
                        ResidentialAddressStreetName = ros[18],
                        ResidentialAddressSuburb = ros[19],
                        ResidentialAddressPOcode = ros[20],
                        ResidentialAddressProvince = ros[21]
                    };
                    if (ros[22] == "Y")
                    {
                        cl.HomeAssistPanicSOS = true;
                    }
                    else
                    {
                        cl.HomeAssistPanicSOS = false;
                    }
                    AllClients.Add(cl);																										
				}
			}
            database.BulkInsert(AllClients);


			//string csvData = File.ReadAllText(path);
			//csvData = csvData.Substring(csvData.IndexOf(Environment.NewLine) + 1);
			//DataTable dt = new DataTable();
			//dt.Columns.AddRange(new DataColumn[24] {
			//    new DataColumn("IDVal",typeof(int)),
			//    new DataColumn("MemberStatus",typeof(string)),
			//    new DataColumn("PolicyType",typeof(string)),
			//    new DataColumn("MemberNo",typeof(string)),
			//    new DataColumn("Policy_Inception_date",typeof(DateTime)),
			//    new DataColumn("CancellationDate",typeof(DateTime)),
			//    new DataColumn("Title",typeof(string)),
			//    new DataColumn("Name",typeof(string)),
			//    new DataColumn("Surname",typeof(string)),
			//    new DataColumn("IDNumber",typeof(string)),
			//    new DataColumn("PassportNumber",typeof(string)),
			//    new DataColumn("TelHome",typeof(string)),
			//    new DataColumn("TelWork",typeof(string)),
			//    new DataColumn("TelOther",typeof(string)),
			//    new DataColumn("MainEmailAddress",typeof(string)),
			//    new DataColumn("AltEmailAddress",typeof(string)),
			//    new DataColumn("ResidentialAddressComplexNo",typeof(string)),
			//    new DataColumn("ResidentialAddressComplexName",typeof(string)),
			//    new DataColumn("ResidentialAddressStreetNo",typeof(string)),
			//    new DataColumn("ResidentialAddressStreetName",typeof(string)),
			//    new DataColumn("ResidentialAddressSuburb",typeof(string)),
			//    new DataColumn("ResidentialAddressPOcode", typeof(string)),
			//    new DataColumn("ResidentialAddressProvince",typeof(string)),
			//    new DataColumn("HomeAssistPanicSOS",typeof(bool))});
			//foreach (string row in csvData.Split(Environment.NewLine.ToCharArray()))
			//{
			//    if (!string.IsNullOrEmpty(row))
			//    {
			//        dt.Rows.Add();
			//        int i = 1;
			//        foreach (string cell in row.Split('|'))
			//        {
			//            try
			//            {
			//                dt.Rows[dt.Rows.Count - 1][i] = cell;
			//                i++;
			//            }
			//            catch (Exception ex)
			//            {
			//                if (ex.Message.Contains("Expected type is Boolean"))
			//                {
			//                    if (cell == "Y")
			//                    {
			//                        dt.Rows[dt.Rows.Count - 1][i] = true;
			//                        i++;
			//                    }
			//                    else
			//                    {
			//                        dt.Rows[dt.Rows.Count - 1][i] = false;
			//                        i++;
			//                    }
			//                }
			//            }
			//        }
			//    }
			//}
			//if (dt.Rows.Count > 0)
			//{
			//    RemoveTableData();
			//}

			//using (SqlConnection con = new SqlConnection(SQLConnectionParm))
			//{
			//    con.Open();
			//    SqlBulkCopy bulk = new SqlBulkCopy(con);
			//    bulk.DestinationTableName = "dbo.tblCustomer";
			//    bulk.WriteToServer(dt);
			//    con.Close();
			//}           
		}
		private void RemoveTableData()
        {
            database.DeleteTableData();
			//try
			//{
   //             using (SqlConnection con = new SqlConnection(SQLConnectionParm))
   //             {
   //                 con.Open();
   //                 cmd.Connection = con;
   //                 cmd.CommandText = "DELETE FROM tblCustomer";
   //                 cmd.ExecuteNonQuery();
   //                 con.Close();
   //             }
                
   //         }
			//catch
			//{
			//}
          
        }
        private void LoadNewID(string NewId)
        {
            database.LoadNewID(NewId);
			//try
			//{
   //             using (SqlConnection con = new SqlConnection(SQLConnectionParm))
   //             {
   //                 con.Open();
   //                 cmd.Connection = con;
   //                 cmd.CommandText = "DELETE FROM tblID";
   //                 cmd.ExecuteNonQuery();
   //                 cmd.CommandText = "INSERT INTO tblID(FirstID) VALUES ('" + NewId + "')";
   //                 cmd.ExecuteNonQuery();
   //                 con.Close();
   //             }
               
   //         }
			//catch (Exception)
			//{
			//}
            
        }
        private void LoadNewAccessToken(string Token)
        {
            database.LoadNewAccessToken(Token);
			//try
			//{
   //             using (SqlConnection con = new SqlConnection(SQLConnectionParm))
   //             {
   //                 con.Open();
   //                 cmd.Connection = con;
   //                 cmd.CommandText = "DELETE FROM AccessTokens";
   //                 cmd.ExecuteNonQuery();
   //                 cmd.CommandText = "INSERT INTO AccessTokens(AccessToken) VALUES ('" + Token + "')";
   //                 cmd.ExecuteNonQuery();
   //                 con.Close();
   //             }
               
   //         }
			//catch (Exception)
			//{

			//}
            
        }
        private string GetCurrentLoadedID()
        {
            return  database.GetCurrentID();
			//try
			//{
   //             using (SqlConnection con = new SqlConnection(SQLConnectionParm))
   //             {
   //                 con.Open();
   //                 cmd.Connection = con;
   //                 cmd.CommandText = "Select FirstID FROM tblID";
   //                 string currentID = cmd.ExecuteScalar().ToString();
   //                 con.Close();
   //                 return currentID;
   //             }                
   //         }
			//catch (Exception)
			//{
   //             return "";
			//}
           
        }
        private string GetCurrentAccessToken()
        {
            return  database.GetCurrentAccessToken();
			//try
			//{
   //             using (SqlConnection con = new SqlConnection(SQLConnectionParm))
   //             {
   //                 con.Open();
   //                 cmd.Connection = con;
   //                 cmd.CommandText = "Select AccessToken FROM AccessTokens";
   //                 string currentID = cmd.ExecuteScalar().ToString();
   //                 con.Close();
   //                 return currentID;
   //             }               
   //         }
			//catch (Exception)
			//{
   //             return null;
			//}
          
        }
        private void SendLogData(string Rows, string fieldName, string searchVal, string DateTime)
        {
            GoogleCredential credential = GoogleCredential.FromAccessToken( GetCurrentAccessToken());
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
            catch(Exception ex) {
                Response.Write("<script>alert('Unable To send Data " + ex + "')</script>");
            }
        }
        private void GetAccessToken()
		{
            var codeSt = Request.Url.Query;
            if (codeSt.ToString() == "")
            {
                var codeItem = mainSettings.auth_uri + "" + "?client_id=" + mainSettings.client_id + "&scope=" + SheetsService.Scope.Spreadsheets + "&access_type=offline" + "&redirect_uri=" + mainSettings.redirect_uris[0] + "&response_type=code";
                Response.Redirect(codeItem);
            }
            string queryString = codeSt.ToString();
            char[] delimiterChars = { '=' };
            string[] words = queryString.Split(delimiterChars);
            string code = words[1];

            string formData = "client_id=" + mainSettings.client_id + "&client_secret=" + mainSettings.client_secret + "&redirect_uri=" + mainSettings.redirect_uris[0] + "&grant_type=authorization_code" + "&code=" + code;

            var exchangeWC = new WebClient();
            exchangeWC.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var results = exchangeWC.UploadString(new Uri(mainSettings.token_uri), formData);

            var tokenData = JsonConvert.DeserializeObject<QuickSearch2.AccessToken>(results);
            var token = new TokenResponse
            {
                AccessToken = tokenData.access_token
            };
            LoadNewAccessToken(tokenData.access_token);
        }
        private void GetFileId()
        {
            
            GoogleCredential credential =GoogleCredential.FromAccessToken( GetCurrentAccessToken());

            var service = new SheetsService(new BaseClientService.Initializer()
            {             
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            try
            {
                String spreadsheetId = QuickSearch2.Properties.Settings.Default.DefaultSpreadsheetID;
                String range = "Sheet1!A1";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange ress = request.Execute();
                IList<IList<Object>> values = ress.Values;
                if (values != null && values.Count > 0)
                {
                    var mainID = values[0];
                    fileId = mainID[0].ToString();
                }
                else
                {
                    Response.Write("<script>alert('Could not find ID')</script>");
                }
            }
            catch(Exception ex)
            {
                Response.Write("<script>alert('Unable to connect to specified Google Sheet due to "+ex+"')</script>");
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            DropDownList1.SelectedIndex = 0;
            txfSearch.Text = "";
        }
    }

}

