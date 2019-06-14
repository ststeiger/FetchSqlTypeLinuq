using System;

namespace HarvestSqlServerTypes
{
    class Program
    {
    
    
    
        public static void SaveAssembly(string assemblyName, string path)
        {
            string sql = @"
--DECLARE @__assemblyname nvarchar(260)
--SET @__assemblyname = 'Microsoft.SqlServer.Types'
SELECT 
	 A.name
	,AF.content 
FROM sys.assembly_files AS AF 
INNER JOIN sys.assemblies AS A 
	ON AF.assembly_id = A.assembly_id 
	
WHERE AF.file_id = 1 
AND A.name = @__assemblyname
;
";


            AnySqlWebAdmin.SqlService service = new AnySqlWebAdmin.SqlService();
            

            using (System.Data.Common.DbConnection con = service.Connection)
            {


                using (System.Data.Common.DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;

                    
                    var p = cmd.CreateParameter();
                    p.ParameterName = "__assemblyname";
                    p.DbType = System.Data.DbType.String;
                    p.Value = assemblyName;

                    cmd.Parameters.Add(p);


                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();

                    using (System.Data.IDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        //SqlBytes bytes = reader.GetSqlBytes(0);
                        const int BUFFER_SIZE = 1024;
                        byte[] buffer = new byte[BUFFER_SIZE];

                        int col = reader.GetOrdinal("content");
                        int bytesRead = 0;
                        int offset = 0;

                        // write the byte stream out to disk
                        using (System.IO.FileStream bytestream = new System.IO.FileStream(path, System.IO.FileMode.CreateNew))
                        {

                            while ((bytesRead = (int)reader.GetBytes(col, offset, buffer, 0, BUFFER_SIZE)) > 0)
                            {
                                bytestream.Write(buffer, 0, bytesRead);
                                offset += bytesRead;
                            } // Whend

                            bytestream.Close();
                        } // End Using bytestream 

                        reader.Close();
                    } // End Using reader

                    if (con.State != System.Data.ConnectionState.Closed)
                        con.Close();
                }
                
            }
            
        } // End Function SaveAssembly
        
        
        static void Main(string[] args)
        {
            // SaveAssembly("Microsoft.SqlServer.Types", @"D:\Microsoft.SqlServer.Types.dll");
            SaveAssembly("Microsoft.SqlServer.Types", @"/root/mygithub/Microsoft.SqlServer.Types.dll");
            
            System.Console.WriteLine("Hello World!");
        }
    }
}