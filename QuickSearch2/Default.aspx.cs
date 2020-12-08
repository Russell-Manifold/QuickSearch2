using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2.Responses;
using QuickSearch2.Models;
using System.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace QuickSearch2
{
    public partial class Default : System.Web.UI.Page
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets, DriveService.Scope.Drive};
        static string ApplicationName = "QuickSearchProject";
        static AccessLayer database=null;
        SettingsInfo mainSettings=null;
        string permissionId = "";
        //protected string googleplus_redirect_url = "https://localhost:44322/Default.aspx";
        //protected string googleplus_redirect_url = "http://localhost/QuickSearch";
        private string fileId = "";
        private void LoadJson()
		{
			if (mainSettings==null)
			{
                var s = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("credentials.json"));
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
        private void AllowPermission(string fieldID)
		{
            GoogleCredential credential = GoogleCredential.FromAccessToken(GetCurrentAccessToken());
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            Permission perm = new Permission();
            perm.Type = "anyone";
            perm.Role = "writer";
            permissionId= perm.Id;
			try
			{
                var permss = service.Permissions.Create(perm, fileId).Execute();
                permissionId = permss.Id;
			}
			catch
			{

			}                                
        }
        private void DisablePermission(string fieldID)
        {
            GoogleCredential credential = GoogleCredential.FromAccessToken(GetCurrentAccessToken());
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            try
            {
                service.Permissions.Delete(fileId,permissionId).Execute();
            }
            catch
            {

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
                                AllowPermission(fileId);

                                client.DownloadFile("https://drive.google.com/uc?export=download&id=" + fileId, HttpContext.Current.Server.MapPath(fileId + ".csv"));
                                DisablePermission(fileId);
                                LoadNewID(fileId);
                                LoadCSVToDb(HttpContext.Current.Server.MapPath(fileId + ".csv"));
								try
								{
									System.IO.File.Delete(HttpContext.Current.Server.MapPath(fileId + ".csv"));
                                }
								catch{ }                                
                            }
                            catch(Exception ex) {
                                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Could not download file " + ex + "')", true);
                            }
                        }
                    }
                }
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Please select a field')", true);
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
                    }
                    if (output.Count > 0)
					{
                        try
                        {
                            SendLogData(output.Count + "", Field, SearchVal, DateTime.Now.ToString("dd MMM yyyy hh:mm:ss"));
                        }
                        catch (Exception ex)
                        {
                           ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Unable To send Data " + ex + "')", true);
                        }
                     }
				}
                catch (Exception ex)
                {
                   ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Could not load due to " + ex + "')", true);
                }
            }
        }
        private void LoadCSVToDb(string path)
        {
			string csvData = System.IO.File.ReadAllText(path);
			csvData = csvData.Substring(csvData.IndexOf(Environment.NewLine) + 1);
            List<Client> AllClients = new List<Client>();
            string[] ros = new string[24];
            foreach (string row in csvData.Split(Environment.NewLine.ToCharArray()))
			{            
				if (!string.IsNullOrEmpty(row))
				{
					try
					{
                        ros = row.Split('|');
                        Client cl = new Client
                        {
                            MemberStatus = ros[0],
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
					catch
					{

					}
                   																								
				}
			}
            database.DeleteTableData();
            database.BulkInsert(AllClients);			           
		}
		private void RemoveTableData()
        {
            database.DeleteTableData();          
        }
        private void LoadNewID(string NewId)
        {
            database.LoadNewID(NewId);
			
            
        }
        private void LoadNewAccessToken(string Token)
        {
            database.LoadNewAccessToken(Token);
			
        }
        private string GetCurrentLoadedID()
        {
            return  database.GetCurrentID();
			
           
        }
        private string GetCurrentAccessToken()
        {
            return  database.GetCurrentAccessToken();
			
        }
        private void SendLogData(string Rows, string fieldName, string searchVal, string DateTime)
        {
            GoogleCredential credential = GoogleCredential.FromAccessToken( GetCurrentAccessToken());
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
          
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
        private void GetAccessToken()
		{
            var codeSt = Request.Url.Query;
            if (codeSt.ToString() == "")
            {
                var codeItem = mainSettings.auth_uri + "" + "?client_id=" + mainSettings.client_id + "&scope=" + "https://www.googleapis.com/auth/spreadsheets  https://www.googleapis.com/auth/drive" + "&access_type=offline" + "&redirect_uri=" + mainSettings.redirect_uris[0] + "&response_type=code";
                Response.Redirect(codeItem);
            }
            string queryString = codeSt.ToString();
            char[] delimiterChars = { '=' };
            string[] words = queryString.Split(delimiterChars);
            string code = words[1];

            string formData = "client_id=" + mainSettings.client_id + "&client_secret=" + mainSettings.client_secret + "&redirect_uri=" + mainSettings.redirect_uris[0] + "&grant_type=authorization_code" + "&code=" + code;

            var exchangeWC = new WebClient();
            exchangeWC.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            try
            {
                var results = exchangeWC.UploadString(new Uri(mainSettings.token_uri), formData);
                var tokenData = JsonConvert.DeserializeObject<QuickSearch2.AccessToken>(results);
                var token = new TokenResponse
                {
                    AccessToken = tokenData.access_token
                };
                LoadNewAccessToken(tokenData.access_token);
            }          
            catch { 
            }                        
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
                    ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Could not find ID')", true);
                }
            }
            catch(Exception ex)
            {
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "Error", "alert('Unable to connect to specified Google Sheet due to " + ex + "')", true);
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            DropDownList1.SelectedIndex = 0;
            txfSearch.Text = "";
        }
    }

}

