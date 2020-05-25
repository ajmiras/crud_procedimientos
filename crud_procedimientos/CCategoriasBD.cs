using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace crud_procedimientos
{
    class CCategoriasBD
    {
        private CConexionBD conexionBD = new CConexionBD();
        private SqlCommand sqlCommand = new SqlCommand();
        private SqlDataReader sqlDataReader;

        public int Categoria_id { get; set; }
        public String Categoria { get; set; }

        public DataTable Seleccionar(int categoria_id = 0)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                // Esto cambia con respecto a trabajar con sentencias SQL.
                sqlCommand.CommandType = CommandType.StoredProcedure; 
                sqlCommand.CommandText = "CategoriasSeleccionar";
                sqlCommand.Parameters.AddWithValue("@categoria_id", categoria_id);

                sqlDataReader = sqlCommand.ExecuteReader();

                dataTable.Load(sqlDataReader);

                if ((categoria_id != 0) &&
                    (dataTable.Rows.Count != 0))
                {
                    DataRow[] rows = dataTable.Select();

                    Categoria_id = categoria_id;
                    Categoria = rows[0]["categoría"].ToString();
                }
            }
            finally
            {
                sqlCommand.Parameters.Clear();
                sqlDataReader.Close();
                conexionBD.Cerrar();
            }

            return dataTable;
        }
    }
}
