using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace crud_procedimientos
{
    class CMarcasBD
    { 
        private CConexionBD conexionBD = new CConexionBD();
        private SqlCommand sqlCommand = new SqlCommand();
        private SqlDataReader sqlDataReader;
        private String sError = "";

        public int Marca_id { get; set; }
        public String Marca { get; set; }
        public int Codigo { get; set; }
        public String Error { get; }

        public DataTable Seleccionar(int marca_id = 0)
        {
            DataTable dataTable = new DataTable();
            try
            {
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "MarcasSeleccionar";
                sqlCommand.Parameters.AddWithValue("@marca_id", marca_id);

                sqlDataReader = sqlCommand.ExecuteReader();

                dataTable.Load(sqlDataReader);

                if ((marca_id != 0) && (dataTable.Rows.Count != 0))
                {
                    DataRow[] rows = dataTable.Select();

                    Marca_id = marca_id;
                    Marca = rows[0]["marca"].ToString();
                    Codigo = Convert.ToInt32(rows[0]["codigo"]);
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

        public bool Insertar()
        {
            bool bInsertada = false;

            sError = "";

            try
            {
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "MarcasInsertar";
                sqlCommand.Parameters.AddWithValue("@marca_id", Marca_id);
                sqlCommand.Parameters.AddWithValue("@marca", Marca);
                sqlCommand.Parameters.AddWithValue("@codigo", Codigo);

                var returnParameter = sqlCommand.Parameters.Add("@marca_id", SqlDbType.Int);

                returnParameter.Direction = ParameterDirection.ReturnValue;

                bInsertada = sqlCommand.ExecuteNonQuery() == 1;

                if (bInsertada)
                    Marca_id = Convert.ToInt32(returnParameter.Value);
            }
            catch (Exception ex)
            {
                sError = "Código duplicado.\n\n" + ex.Message;

                bInsertada = false;
            }
            finally
            {
                sqlCommand.Parameters.Clear();

                conexionBD.Cerrar();
            }

            return bInsertada;
        }
        public bool Borrar()
        {
            bool bBorrada = false;

            try
            {
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "MarcasBorrar";
                sqlCommand.Parameters.AddWithValue("@marca_id", Marca_id);

                bBorrada = sqlCommand.ExecuteNonQuery() == 1;
            }
            finally
            {
                conexionBD.Cerrar();
            }

            return bBorrada;
        }

        public bool Editar()
        {
            bool bEditada = false;

            sError = "";

            try
            {
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "MarcasEditar";
                sqlCommand.Parameters.AddWithValue("@marca_id", Marca_id);
                sqlCommand.Parameters.AddWithValue("@marca", Marca);
                sqlCommand.Parameters.AddWithValue("@codigo", Codigo);

                bEditada = sqlCommand.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                sError = "Código duplicado.\n\n" + ex.Message;

                bEditada = false;
            }
            finally
            {
                sqlCommand.Parameters.Clear();
                conexionBD.Cerrar();
            }

            return bEditada;
        }
    }
}
