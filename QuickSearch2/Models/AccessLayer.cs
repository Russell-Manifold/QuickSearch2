using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QuickSearch2.Models
{
	public class AccessLayer
	{
		readonly SQLiteConnection database;

        public AccessLayer(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<AToken>();
            database.CreateTable<FieldID>();
            database.CreateTable<Client>();
        }

        public int LoadNewAccessToken(string token)
		{
            database.Table<AToken>().Delete(x=>x.ID>0);
            return database.Insert(new AToken { FullToken=token});
		}
        public string GetCurrentAccessToken()
		{
            return (database.Table<AToken>().FirstOrDefault()).FullToken;
		}
        public int LoadNewID(string IDS)
		{
            database.Table<FieldID>().Delete(x => x.ID > 0);
            return database.Insert(new FieldID { FieldId= IDS });
		}
        public string GetCurrentID()
		{
            FieldID f = database.Table<FieldID>().FirstOrDefault();
			if (f==null)
			{
                return "";
			}
            return f.FieldId;
		}
        public  int BulkInsert(List<Client> allClients)
		{
           return  database.InsertAll(allClients);			
		}
        public List<Client> FetchQuery(string Qry)
		{
           return database.Query<Client>(Qry);
		}
        public int DeleteTableData()
		{
            return database.Table<Client>().Delete(x=>x.IDNumber!=null);
		}
    }
}