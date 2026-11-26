using System.Data.Common;
using System.Net;

namespace Verademo.Commands
{
    public class RemoveAccountCommand : BlabberCommandBase, IBlabberCommand
    {
        public RemoveAccountCommand(DbConnection connect)
        {
            this.connect = connect;
        }

        public void Execute(string blabberUsername)
        {
            var sqlQuery = "DELETE FROM listeners WHERE blabber=@Username OR listener=@Username;";
            logger.Info(sqlQuery);
            DbCommand action;
            action = connect.CreateCommand();
            action.CommandText = sqlQuery;

            var parameter = action.CreateParameter();
            parameter.ParameterName = "@Username";
            parameter.Value = blabberUsername;
            action.Parameters.Add(parameter);

            action.ExecuteNonQuery();

            sqlQuery = "SELECT blab_name FROM users WHERE username = @Username";
            var sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var selectParameter = sqlStatement.CreateParameter();
            selectParameter.ParameterName = "@Username";
            selectParameter.Value = blabberUsername;
            sqlStatement.Parameters.Add(selectParameter);

            logger.Info("SELECT blab_name FROM users WHERE username = " + WebUtility.UrlEncode(blabberUsername));
            var result = sqlStatement.ExecuteScalar();

            /* START BAD CODE */
            var removeEvent = "Removed account for blabber " + result;
            sqlQuery = "INSERT INTO users_history (blabber, event) VALUES (@Blabber, @Event)";
            sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var blabberParam = sqlStatement.CreateParameter();
            blabberParam.ParameterName = "@Blabber";
            blabberParam.Value = blabberUsername;
            sqlStatement.Parameters.Add(blabberParam);

            var eventParam = sqlStatement.CreateParameter();
            eventParam.ParameterName = "@Event";
            eventParam.Value = removeEvent;
            sqlStatement.Parameters.Add(eventParam);

            logger.Info("INSERT INTO users_history (blabber, event) VALUES (" + WebUtility.UrlEncode(blabberUsername) + ", " + WebUtility.UrlEncode(removeEvent) + ")");
            sqlStatement.ExecuteNonQuery();

            sqlQuery = "DELETE FROM users WHERE username = @Username";
            sqlStatement = connect.CreateCommand();
            sqlStatement.CommandText = sqlQuery;

            var deleteParameter = sqlStatement.CreateParameter();
            deleteParameter.ParameterName = "@Username";
            deleteParameter.Value = blabberUsername;
            sqlStatement.Parameters.Add(deleteParameter);

            logger.Info("DELETE FROM users WHERE username = " + WebUtility.UrlEncode(blabberUsername));
            sqlStatement.ExecuteNonQuery();
            /* END BAD CODE */
        }
    }
}