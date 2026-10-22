namespace Qahr.Mobile.DBFunctionality
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    // [net10] MySqlConnector in the .NET 10 build (Qahr.Mobile.Net10.csproj); MySql.Data in the 4.8 build.
    using MYSQLCOMMAND = MySqlConnector.MySqlCommand;
    using MYSQLREADER = MySqlConnector.MySqlDataReader;
    using MYSQLCONNECTION = MySqlConnector.MySqlConnection;


    public unsafe class MySqlCommand : IDisposable
    {
        private MySqlCommandType _type;

        public MySqlCommandType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        protected StringBuilder _command;

        public String Command
        {
            get { return _command.ToString(); }
            set { _command = new StringBuilder(value); }
        }

        private bool firstPart = true;

        private System.SafeDictionary<Byte, String> insertFields;
        private System.SafeDictionary<Byte, String> insertValues;
        private Byte lastpair;

        public MySqlCommand(MySqlCommandType Type)
        {
            this.Type = Type;
            switch (Type)
            {
                case MySqlCommandType.SELECT: _command = new StringBuilder("SELECT * FROM <R>"); break;
                case MySqlCommandType.UPDATE: _command = new StringBuilder("UPDATE <R> SET "); break;
                case MySqlCommandType.INSERT:
                    {
                        insertFields = new System.SafeDictionary<Byte, String>();
                        insertValues = new System.SafeDictionary<Byte, String>();
                        lastpair = 0;
                        _command = new StringBuilder("INSERT INTO <R> (<F>) VALUES (<V>)");
                        break;
                    }
                case MySqlCommandType.DELETE: _command = new StringBuilder("DELETE FROM <R> WHERE <C> = <V>"); break;
                case MySqlCommandType.COUNT: _command = new StringBuilder("SELECT count(<V>) FROM <R>"); break;
            }
        }
        public int Execute2()
        {
            if (Type == MySqlCommandType.INSERT)
            {
                string fields = "";
                string values = "";
                byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += "`" + insertFields[x] + "`,";
                    else
                        fields += "`" + insertFields[x] + "`";
                    #endregion
                    #region Values
                    if (comma)
                        values += "'" + insertValues[x] + "'" + ",";
                    else
                        values += "'" + insertValues[x] + "'";
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }

            using (var conn = DataHolder.MySqlConnection)
            {
                conn.ConnectionString = DataHolder.ConnectionString;
                conn.Open();
                MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command + ";", conn);
                return cmd.ExecuteNonQuery();
            }
        }
        private bool Comma()
        {
            if (firstPart)
            {
                firstPart = false;
                return false;
            }
            String command = _command.ToString();
            if (command[command.Length - 1] == ',' || command[command.Length - 2] == ',' || command[command.Length - 3] == ',')
                return false;
            return true;
        }

        #region Select
        public MySqlCommand Select(String table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion

        #region Count
        public MySqlCommand Count(String table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        #endregion

        #region Delete
        public MySqlCommand Delete(String table, String column, String value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", "'" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Delete(String table, String column, Int64 value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(String table, String column, UInt64 value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", value.ToString());
            return this;
        }
        public MySqlCommand Delete(String table, String column, bool value)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            _command = _command.Replace("<C>", "`" + column + "`");
            _command = _command.Replace("<V>", (value ? "1" : "0"));
            return this;
        }

        #endregion

        #region Update
        public MySqlCommand Update(String table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Set(String column, Int64 value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                else
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
            }
            return this;
        }
        public MySqlCommand Set(String column, Double value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                else
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
            }
            return this;
        }
        public MySqlCommand Set(String column, UInt64 value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + value.ToString() + " ");
                else
                    _command = _command.Append("`" + column + "` = " + value.ToString() + " ");
            }
            return this;
        }
        public MySqlCommand Set(String column, String value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = '" + value.MySqlEscape() + "' ");
                else
                    _command = _command.Append("`" + column + "` = '" + value.MySqlEscape() + "' ");
            }
            return this;
        }
        public MySqlCommand Set(String column, Boolean value)
        {
            if (Type == MySqlCommandType.UPDATE)
            {
                if (Comma())
                    _command = _command.Append(",`" + column + "` = " + (value ? "1" : "0") + " ");
                else
                    _command = _command.Append("`" + column + "` = " + (value ? "1" : "0") + " ");
            }
            return this;
        }
        public MySqlCommand Set(String column, Object value)
        {
            if (value is bool) Set(column, (bool)value);
            else Set(column, value.ToString());
            return this;
        }
        #endregion

        #region Insert
        public MySqlCommand Insert(String table)
        {
            _command = _command.Replace("<R>", "`" + table + "`");
            return this;
        }
        public MySqlCommand Insert(String field, Int64 value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(String field, UInt64 value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(String field, bool value)
        {
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, (value ? 1 : 0).ToString());
            lastpair++;
            return this;
        }
        public MySqlCommand Insert(String field, String value)
        {
            var array = value.ToCharArray();
            String str = global::Qahr.Mobile.AnsiText.Value.GetString(Encoding.Unicode.GetBytes(array, 0, array.Length));
            insertFields.Add(lastpair, field);
            insertValues.Add(lastpair, value.MySqlEscape());
            lastpair++;
            return this;
        }
        #endregion

        #region Where
        public MySqlCommand Where(String column, Int64 value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Where(String column, Int64 value, bool greater)
        {
            if (greater)
                _command = _command.Append("WHERE `" + column + "` > " + value);
            else
                _command = _command.Append("WHERE `" + column + "` < " + value);
            return this;
        }
        public MySqlCommand Where(String column, UInt64 value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Where(String column, String value)
        {
            _command = _command.Append("WHERE `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Where(String column, bool value)
        {
            _command = _command.Append("WHERE `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        #endregion

        #region And
        public MySqlCommand And(String column, Int64 value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand And(String column, UInt64 value)
        {
            _command = _command.Append(" AND `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand And(String column, String value)
        {
            _command = _command.Append(" AND `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand And(String column, bool value)
        {
            _command = _command.Append(" AND `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        public MySqlCommand And(String column, Int64 value, bool greater)
        {
            if (greater)
                _command = _command.Append(" AND `" + column + "` > " + value);
            else
                _command = _command.Append(" AND `" + column + "` < " + value);
            return this;
        }
        #endregion

        #region Or
        public MySqlCommand Or(String column, Int64 value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Or(String column, UInt64 value)
        {
            _command = _command.Append(" Or `" + column + "` = " + value);
            return this;
        }
        public MySqlCommand Or(String column, String value)
        {
            _command = _command.Append(" Or `" + column + "` = '" + value.MySqlEscape() + "'");
            return this;
        }
        public MySqlCommand Or(String column, bool value)
        {
            _command = _command.Append(" Or `" + column + "` = " + (value ? "1" : "0"));
            return this;
        }
        #endregion

        #region Order
        public MySqlCommand Order(String column)
        {
            _command = _command.Append("ORDER BY " + column + "");
            return this;
        }
        #endregion

        public Int32 Execute()
        {
            using (var conn = DataHolder.MySqlConnection)
            {
                conn.Open();
                return Execute(conn);
            }
        }
        public Int32 Execute(MYSQLCONNECTION conn)
        {
            if (Type == MySqlCommandType.INSERT)
            {
                String fields = "";
                String values = "";
                Byte x;
                for (x = 0; x < lastpair; x++)
                {
                    bool comma = (x + 1) == lastpair ? false : true;
                    #region Fields
                    if (comma)
                        fields += "`" + insertFields[x] + "`,";
                    else
                        fields += "`" + insertFields[x] + "`";
                    #endregion
                    #region Values
                    if (comma)
                        values += "'" + insertValues[x] + "'" + ",";
                    else
                        values += "'" + insertValues[x] + "'";
                    #endregion
                }
                _command = _command.Replace("<F>", fields);
                _command = _command.Replace("<V>", values);
            }

            MYSQLCOMMAND cmd = new MYSQLCOMMAND(Command, conn);
            return cmd.ExecuteNonQuery();
        }

        public MySqlReader CreateReader()
        {
            return new MySqlReader(this);
        }

        void IDisposable.Dispose()
        {
            if (insertValues != null)
            {
                insertValues.Clear();
                insertFields.Clear();
            }
            _command = null;
        }
    }
    public enum MySqlCommandType
    {
        DELETE, INSERT, SELECT, UPDATE, COUNT
    }

}
