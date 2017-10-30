using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace DataAbstraction
{
    /**
     * @brief   Base class of objects that abstract database table records.
     * @author  Matt Drage
     * @date    21/06/2012
     */
    public class DatabaseObject
    {
        protected SqlConnection connection = new SqlConnection();
        private SqlCommand sqlSave;
        private SqlCommand sqlNew;
        private SqlCommand sqlExists;
        private SqlCommand sqlLoad;
        private SqlCommand sqlDelete;

        /**
         * @brief   Gets or sets the database connection string.
         * @return  The connection string.
         */
        public virtual string Connection
        {
            get { return connection.ConnectionString; }
            set { connection.ConnectionString = value; }
        }

        /**
         * @brief   Default constructor.
         * @author  Matt Drage
         * @date    21/06/2012
         */
        public DatabaseObject()
        {
            InitSQL();
        }

        /**
         * @brief   Constructor.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   connection  The database connection.
         */
        public DatabaseObject(string connection)
        {
            Connection = connection;
            InitSQL();
        }

        private void InitSQL()
        {
            // Get database table and field names
            string table = GetTableName();
            string[] fields = GetFieldNames();
            string primaryKey = GetPrimaryKeyName();

            // Construct query to update a record
            string sqlSave = "UPDATE " + table + " SET ";
            foreach (string field in fields)
                sqlSave += field + "=@" + field + ", ";
            sqlSave = sqlSave.Remove(sqlSave.Length - 2); // Remove last comma
            sqlSave += " WHERE " + primaryKey + "=@" + primaryKey;
            this.sqlSave = new SqlCommand(sqlSave, connection);

            // Construct query to insert a new record
            string sqlNew = "INSERT INTO " + table + " (";
            foreach (string field in fields)
                sqlNew += field + ", ";
            sqlNew = sqlNew.Remove(sqlNew.Length - 2);
            sqlNew += ") VALUES (";
            foreach (string field in fields)
                sqlNew += "@" + field + ", ";
            sqlNew = sqlNew.Remove(sqlNew.Length - 2);
            sqlNew += ")";
            this.sqlNew = new SqlCommand(sqlNew, connection);

            // Construct query to check if a record exists
            string sqlExists = "SELECT " + primaryKey + " FROM " + table + " WHERE " + primaryKey + "=@" + primaryKey;
            this.sqlExists = new SqlCommand(sqlExists, connection);

            // Construct query to load a record
            string sqlLoad = "SELECT ";
            sqlLoad += primaryKey + ", ";
            foreach (string field in fields)
                sqlLoad += field + ", ";
            sqlLoad = sqlLoad.Remove(sqlLoad.Length - 2);
            sqlLoad += " FROM " + table + " WHERE " + primaryKey + "=@" + primaryKey;
            this.sqlLoad = new SqlCommand(sqlLoad, connection);

            // Construct query to delete a record
            string sqlDelete = "DELETE FROM " + table + " WHERE " + primaryKey + "=@" + primaryKey;
            this.sqlDelete = new SqlCommand(sqlDelete, connection);
        }

        /**
         * @brief   Gets the name of the database table that the DBO belongs to.
         * @author  Matt Drage
         * @date    21/06/2012
         * @return  The table name.
         */
        private string GetTableName()
        {
            object[] classAttributes = this.GetType().GetCustomAttributes(true);
            foreach (object attr in classAttributes)
            {
                if (attr is DataTable)
                {
                    DataTable table = (DataTable)attr;
                    return table.Name;
                }
            }
            return null;
        }

        /**
         * @brief   Gets the field names of the DBO (not including the primary key).
         * @author  Matt Drage
         * @date    21/06/2012
         * @return  The field names.
         */
        private string[] GetFieldNames()
        {
            List<string> fields = new List<string>();
            MemberInfo[] members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo member in members)
            {
                object[] attributes = member.GetCustomAttributes(typeof(DataField), true);
                if (attributes.Length > 0)
                {
                    if (attributes[0] is DataField)
                    {
                        DataField field = (DataField)attributes[0];
                        fields.Add(field.Name);
                    }
                }
            }
            return fields.ToArray();
        }

        /**
         * @brief   Gets the primary key name of the DBO.
         * @author  Matt Drage
         * @date    21/06/2012
         * @return  The primary key name.
         */
        private string GetPrimaryKeyName()
        {
            MemberInfo[] members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo member in members)
            {
                object[] attributes = member.GetCustomAttributes(typeof(PrimaryKey), true);
                if (attributes.Length > 0)
                {
                    if (attributes[0] is PrimaryKey)
                    {
                        PrimaryKey pk = (PrimaryKey)attributes[0];
                        return pk.Name;
                    }
                }
            }
            return null;
        }

        /**
         * @brief   Gets the value of a DBO property from the name of the corresponding db field.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   fieldName   Name of the database table field.
         * @return  The property value.
         */
        private object GetPropertyValue(string fieldName)
        {
            MemberInfo[] members = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MemberInfo m = null;
            foreach (MemberInfo member in members)
            {
                object[] attributes = member.GetCustomAttributes(typeof(Attribute), true);
                if (attributes.Length > 0)
                {
                    if (attributes[0] is PrimaryKey)
                    {
                        if (((PrimaryKey)attributes[0]).Name == fieldName)
                        {
                            m = member;
                            break;
                        }
                    }
                    else if (attributes[0] is DataField)
                    {
                        if (((DataField)attributes[0]).Name == fieldName)
                        {
                            m = member;
                            break;
                        }
                    }
                }
            }
            if (m == null)
                return null;
            else
                return this.GetType().GetProperty(m.Name).GetValue(this, null);
        }

        /**
         * @brief   Gets the value of a property.
         * @author  Matt Drage
         * @date    5/07/2012
         * @param   propertyName    Name of the property.
         * @return  The value.
         */
        public object GetValue(string propertyName)
        {
            return this.GetType().GetProperty(propertyName).GetValue(this, null);
        }

        /**
         * @brief   Sets the value of a property.
         * @author  Matt Drage
         * @date    5/07/2012
         * @param   propertyName    Name of the property.
         * @param   value           The value.
         */
        public void SetValue(string propertyName, object value)
        {
            this.GetType().GetProperty(propertyName).SetValue(this, value, null);
        }

        /**
         * @brief   Saves this object - updates existing record in database or adds a new record.
         * @author  Matt Drage
         * @date    21/06/2012
         */
        public bool Save()
        {
            try
            {
                string[] fields = GetFieldNames();
                string primaryKey = GetPrimaryKeyName();

                 // Check if the record already exists
                sqlExists.Parameters.Clear();
                sqlExists.Parameters.AddWithValue("@" + primaryKey, GetPropertyValue(primaryKey));
                connection.Open();
                if (sqlExists.ExecuteScalar() == null)
                {
                    // Insert new record
                    sqlNew.Parameters.Clear();
                    sqlNew.Parameters.AddWithValue("@" + primaryKey, GetPropertyValue(primaryKey));
                    foreach (string field in fields)
                    {
                        object val = GetPropertyValue(field);
                        if (val == null)
                            val = "NULL";
                        sqlNew.Parameters.AddWithValue("@" + field, val);
                    }
                    sqlNew.ExecuteNonQuery();
                }
                else
                {
                    // Update existing record
                    sqlSave.Parameters.Clear();
                    sqlSave.Parameters.AddWithValue("@" + primaryKey, GetPropertyValue(primaryKey));
                    foreach (string field in fields)
                    {
                        object val = GetPropertyValue(field);
                        if (val == null)
                            val = "NULL";
                        sqlSave.Parameters.AddWithValue("@" + field, val);
                    }
                    sqlSave.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
            }
        }

        /**
         * @brief   Loads a DBO given the primary key.
         * @author  Matt Drage
         * @date    21/06/2012
         * @param   primaryKeyValue Primary key identifier of the database record to load.
         * @return  true if it succeeds, false if it fails.
         */
        public bool Load(object primaryKeyValue)
        {
            try
            {
                string[] fields = GetFieldNames();
                string primaryKey = GetPrimaryKeyName();
                string tableName = GetTableName();

                sqlLoad.Parameters.Clear();
                sqlLoad.Parameters.AddWithValue("@" + primaryKey, primaryKeyValue);

                System.Data.DataTable data = new System.Data.DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlLoad);
                adapter.Fill(data);

                if (data.Rows.Count < 1)
                    return false;

                PropertyInfo pkProperty = GetPKProperty();
                pkProperty.SetValue(this, primaryKeyValue, null);

                PropertyInfo[] fieldProperties = GetFieldProperties();
                for (int i = 0; i < fieldProperties.Count(); i++)
                    fieldProperties[i].SetValue(this, data.Rows[0].Field<object>(i + 1), null);

                return true;
            }
            catch (Exception)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
            }
        }

        /**
         * @brief   Deletes the database record this object represents.
         * @author  Matt Drage
         * @date    22/06/2012
         * @return  true if it succeeds, false if it fails.
         */
        public bool Delete()
        {
            try
            {
                string primaryKey = GetPrimaryKeyName();
                sqlDelete.Parameters.Clear();
                sqlDelete.Parameters.AddWithValue("@" + primaryKey, GetPropertyValue(primaryKey));
                connection.Open();
                int rowsEffected = sqlDelete.ExecuteNonQuery();
                connection.Close();
                return (rowsEffected == 1);
            }
            catch
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
            }
        }

        /**
         * @brief   Gets the properties of the DBO that relate to database record fields.
         * @author  Matt Drage
         * @date    21/06/2012
         * @return  The field properties.
         */
        private PropertyInfo[] GetFieldProperties()
        {
            PropertyInfo[] allProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> fields = new List<PropertyInfo>();
            foreach (PropertyInfo property in allProperties)
            {
                if (property.GetCustomAttributes(typeof(DataField), true).Length == 1)
                    fields.Add(property);
            }
            return fields.ToArray();
        }

        /**
         * @brief   Gets the primary key property of the DBO.
         * @author  Matt Drage
         * @date    21/06/2012
         * @return  The primary key property.
         */
        private PropertyInfo GetPKProperty()
        {
            PropertyInfo[] allProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in allProperties)
            {
                if (property.GetCustomAttributes(typeof(PrimaryKey), true).Length == 1)
                    return property;
            }
            return null;
        }

        /**
         * @brief   Gets the name of a database table field name from the related property.
         * @author  Matt Drage
         * @date    22/06/2012
         * @param   property    The property.
         * @return  The field name.
         */
        private static string GetFieldName(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(typeof(Attribute), true);
            if (attributes.Length > 0)
            {
                if (attributes[0] is PrimaryKey)
                    return ((PrimaryKey)attributes[0]).Name;
                else if (attributes[0] is DataField)
                    return ((DataField)attributes[0]).Name;
            }
            return null;
        }

        /**
         * @brief   Loads DBOs from a database table.
         * @author  Matt Drage
         * @date    21/06/2012
         * @tparam  T   Generic type parameter - must inherit from DatabaseObject and define a default constructor.
         * @param   connection  The database connection.
         * @param   filters     (optional) the filters to apply ('WHERE [property] LIKE [value]' SQL equivalent).
         * @param   sort        (optional) the sorting to apply ('ORDER BY [property] ASC|DESC' SQL equivalent).
         * @return  A list of Database Objects.
         */
        public static List<T> Load<T>(string connection, QueryFilter[] filters = null, QuerySort sort = null)
            where T : DatabaseObject, new()
        {
            SqlConnection con = new SqlConnection(connection);

            try
            {
                // Get names from template object
                T template = new T();
                string primaryKey = template.GetPrimaryKeyName();
                string table = template.GetTableName();

                // Create SQL to get PKs
                string sql = "SELECT " + primaryKey + " FROM " + table;

                // Apply filters to SQL string
                if (filters != null)
                {
                    sql += " WHERE ";
                    int count = 0;
                    foreach (QueryFilter filter in filters)
                    {
                        count++;
                        filter.Field = GetFieldName(typeof(T).GetProperty(filter.Property));
                        sql += filter.Field + " ";
                        if (filter.Negation)
                            sql += "NOT ";
                        switch (filter.CompareType)
                        {
                            case QueryFilter.ComparisonType.Equals: sql += "="; break;
                            case QueryFilter.ComparisonType.GreaterThan: sql += ">"; break;
                            case QueryFilter.ComparisonType.LessThan: sql += "<"; break;
                            case QueryFilter.ComparisonType.Like: sql += "LIKE"; break;
                        } 
                        sql += " @f" + count + " AND ";
                    }
                    sql = sql.Remove(sql.Length - 4); // Remove last AND
                }

                // Apply sorting to SQL string
                if (sort != null)
                {
                    sort.Field = GetFieldName(typeof(T).GetProperty(sort.Property));
                    sql += " ORDER BY " + sort.Field;
                    if (sort.Direction == SortDirection.Descending)
                        sql += " DESC";
                }

                // Create query - insert parameterised variables
                SqlCommand sqlGetPKs = new SqlCommand(sql, con);
                if (filters != null)
                {
                    int count = 0;
                    foreach (QueryFilter filter in filters)
                    {
                        count++;
                        sqlGetPKs.Parameters.AddWithValue("@f" + count, filter.Filter);
                    }
                }

                // Get data table of PKs
                con.Open();
                System.Data.DataTable data = new System.Data.DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlGetPKs);
                adapter.Fill(data);
                con.Close();

                // Load objects using PKs
                List<T> objs = new List<T>();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    T obj = new T();
                    obj.Connection = connection;
                    obj.Load(data.Rows[i].Field<object>(0));
                    objs.Add(obj);
                }
                return objs;
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                return null;
            }
        }

        /**
         * @brief   Loads DBOs from a database table using a single filter
         * @author  Matt Drage
         * @date    22/06/2012
         * @tparam  T   Generic type parameter.
         * @param   connection  The database connection.
         * @param   filter      The filter.
         * @return  A list of DBOs.
         */
        public static List<T> Load<T>(string connection, QueryFilter filter)
            where T : DatabaseObject, new()
        {
            QueryFilter[] filters = new QueryFilter[1];
            filters[0] = filter;

            return Load<T>(connection, filters, null);
        }
    }
}
