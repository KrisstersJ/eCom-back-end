using Microsoft.Extensions.Configuration;
using Dapper;
using eCom.Models;
using System.Data;
using System.Data.SqlClient;

namespace eCom.Data.Repositories
{
    public interface IAccountRepository
    {
        bool Register(Registration registration);
        bool Login(Login login);
        string GetFullNameByEmail(string email);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("eCom");
        }

        public bool Register(Registration registration)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string storedProcedure = "RegisterUser";

                var parameters = new DynamicParameters();
                parameters.Add("@FullName", registration.FullName);
                parameters.Add("@Email", registration.Email);
                parameters.Add("@PhoneNumber", registration.PhoneNumber);
                parameters.Add("@Password", registration.Password);

                int rowsAffected = con.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                return rowsAffected > 0;
            }
        }

        public bool Login(Login login)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string storedProcedure = "LoginUser";

                var parameters = new DynamicParameters();
                parameters.Add("@Email", login.Email);
                parameters.Add("@Password", login.Password);
                parameters.Add("@IsValidUser", dbType: DbType.Boolean, direction: ParameterDirection.Output);

                con.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                bool isValidUser = parameters.Get<bool>("@IsValidUser");

                return isValidUser;
            }
        }

        public string GetFullNameByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT FullName FROM Registration WHERE Email = @Email";

                var parameters = new DynamicParameters();
                parameters.Add("@Email", email);

                return con.ExecuteScalar<string>(query, parameters);
            }
        }
    }
}
