using System;
using System.Collections.Generic;
using Acerva.Infra;

namespace Acerva.Modelo.Mapeamento
{
    /// <summary>
    /// Class that represents the Role table in the MySQL Database
    /// </summary>
    public class RoleTable 
    {
        private MySQLDatabase _database;

        /// <summary>
        /// Constructor that takes a MySQLDatabase instance 
        /// </summary>
        /// <param name="database"></param>
        public RoleTable(MySQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Deltes a role from the roles table
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public int Delete(string roleId)
        {
            string commandText = "Delete from roles where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new Role in the roles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public int Insert(Papel role)
        {
            string commandText = "Insert into roles (Id, Name) values (@id, @name)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", role.Name);
            parameters.Add("@id", role.Id);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public string GetRoleName(string roleId)
        {
            string commandText = "Select Name from roles where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public string GetRoleId(string roleName)
        {
            string roleId = null;
            string commandText = "Select Id from roles where Name = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", roleName } };

            var result = _database.QueryValue(commandText, parameters);
            if (result != null)
            {
                return Convert.ToString(result);
            }

            return roleId;
        }

        /// <summary>
        /// Gets the Papel given the role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Papel GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            Papel role = null;

            if(roleName != null)
            {
                role = new Papel(roleName, roleId);
            }

            return role;

        }

        /// <summary>
        /// Gets the Papel given the role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Papel GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            Papel role = null;

            if (roleId != null)
            {
                role = new Papel(roleName, roleId);
            }

            return role;
        }

        public int Update(Papel role)
        {
            string commandText = "Update roles set Name = @name where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", role.Id);

            return _database.Execute(commandText, parameters);
        }
    }
}
