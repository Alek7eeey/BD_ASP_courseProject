using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace TaskWave_MVC.Data
{
    public class Connection
    {
        private readonly IOptions<AppSettings> _appSettings;
        public Connection(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        private string GetConnectionString(Models.ENUMs.RoleENUM role)
        {
            string conStringUser = "";

            switch (role)
            {
                case Models.ENUMs.RoleENUM.USER:
                    {
                        return conStringUser = _appSettings.Value.ConnectionUser;
                    }

                case Models.ENUMs.RoleENUM.ADMIN:
                    {
                        return conStringUser = _appSettings.Value.ConnectionAdmin;
                    }

                case Models.ENUMs.RoleENUM.SUPERUSER:
                    {
                        return conStringUser = _appSettings.Value.ConnectionSuperUser;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        public OracleConnection GetConnection(Models.ENUMs.RoleENUM role)
        {

            //string conStringUser = "User Id=" + user + ";Password=" + pwd + ";Data Source=" + db + ";";

            OracleConnection conn = new OracleConnection(GetConnectionString(role));
            return conn;
        }
    }
}
