using Dapper;
using MasterDataService.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataService.Repository
{
    public abstract class BaseAsyncRepository
    {
        private string sqlwriterConnectionString;
        private string sqlreaderConnectionString;
        private string databaseType;


        public BaseAsyncRepository(IConfiguration configuration)
        {
            sqlwriterConnectionString = configuration.GetSection("DBInfo:WriterConnectionString").Value;
            sqlreaderConnectionString = configuration.GetSection("DBInfo:ReaderConnectionString").Value;
            databaseType = configuration.GetSection("DBInfo:DbType").Value;
        }

        internal DbConnection SqlWriterConnection
        {
            get
            {
                // Initiate Appropriate Database engine specific connection

                // Create IDbConnection as per Database type
                switch (databaseType)
                {
                    case "Postgres":
                        return new NpgsqlConnection(sqlwriterConnectionString);

                    // MySQL
                    case "MySql":
                        return new MySqlConnection(sqlwriterConnectionString);

                    // SQL Server
                    case "SqlServer":
                        return new SqlConnection(sqlwriterConnectionString);

                    // MySQL
                    default:
                        return new MySqlConnection(sqlwriterConnectionString);
                }
            }
        }

        internal DbConnection SqlReaderConnection
        {
            get
            {
                // Initiate Appropriate Database engine specific connection

                // Create IDbConnection as per Database type
                switch (databaseType)
                {
                    case "Postgres":
                        return new NpgsqlConnection(sqlreaderConnectionString);

                    // MySQL
                    case "MySql":
                        return new MySqlConnection(sqlreaderConnectionString);

                    // SQL Server
                    case "SqlServer":
                        return new SqlConnection(sqlreaderConnectionString);

                    // MySQL
                    default:
                        return new MySqlConnection(sqlreaderConnectionString);
                }
            }
        }
    }
}