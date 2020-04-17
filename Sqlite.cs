using System;
using System.Data;
using System.Data.SQLite;
using Tiles.Tools;

namespace tilegrab
{
    public static class Sqlite
    {
        public static bool executeCmd(SQLiteConnection conn, string cmdSql)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = cmdSql.ToString();
                int rowsAffected = cmd.ExecuteNonQuery();
            }

            return true;
        }

        public static void InsertMetadata(SQLiteConnection  conn, Metadata metadata)
        {
            var sql = string.Join(
            "; "
            , $"INSERT INTO metadata (name, value) VALUES ('name', '{metadata.name}');"
            , $"INSERT INTO metadata (name, value) VALUES ('description', '{metadata.description}');"
            , $"INSERT INTO metadata (name, value) VALUES ('bounds', '{metadata.bounds}');"
            , $"INSERT INTO metadata (name, value) VALUES ('center', '{metadata.center}');"
            , $"INSERT INTO metadata (name, value) VALUES ('minzoom', '{metadata.minzoom}');"
            , $"INSERT INTO metadata (name, value) VALUES ('maxzoom', '{metadata.maxzoom}');"
            , $"INSERT INTO metadata (name, value) VALUES ('json', '{metadata.json}');"
            , $"INSERT INTO metadata (name, value) VALUES ('version', '{metadata.version}');"
            , $"INSERT INTO metadata (name, value) VALUES ('type', '{metadata.type}');"
            , "INSERT INTO metadata (name, value) VALUES ('format', 'pbf');"
            );
            Sqlite.executeCmd(conn, sql);
        }

        public static int WriteTile(SQLiteConnection conn, Tile t, byte[] data)
        {
            // mbtiles uses tms format so reverse y-axis...
            var tmsY = Math.Pow(2, t.Z) - 1 - t.Y;
            var cmdInsert = conn.CreateCommand();
            cmdInsert.CommandType = CommandType.Text;
            cmdInsert.CommandText = $"INSERT INTO tiles (zoom_level, tile_column, tile_row, tile_data) VALUES ({t.Z}, {t.X}, {tmsY}, @bytes)";
            cmdInsert.Parameters.AddWithValue("@bytes", data);
            int rowsAffected = cmdInsert.ExecuteNonQuery();
            return rowsAffected;
        }

        public static SQLiteConnection CreateDatabase(string name, string schema)
        {
            string connectionString = $"Data Source={name}";
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            Sqlite.executeCmd(conn, schema);
            conn.Close();
            return conn;
        }
    }
}
