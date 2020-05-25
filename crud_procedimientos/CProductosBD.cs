using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace crud_procedimientos
{
    public class CProductosBD : CConexionBD
    {
        // Para realizar la conexión a la base de datos.
        private CConexionBD conexionBD = new CConexionBD();

        // Para ejecutar un procedimiento almacenado o realizar las sentencias SQL.
        private SqlCommand sqlCommand = new SqlCommand();

        // Para almacenar los datos de una sentencia SELECT.  
        private SqlDataReader sqlDataReader;

        // Variable privada para indicar el error que se ha producido.
        private String sError;

        // Propiedades para almacenar los datos de un registro de la tabla.
        public int Producto_id { get; set; }
        public int Categoria_id { get; set; }
        public String Categoria { get; set; }
        public int Marca_id { get; set; }
        public String Marca { get; set; }
        public String Producto { get; set; }
        public double Precio { get; set; }
        public int Codigo { get; set; }

        // Propiedad que devuelve el error producido. 
        // Al poner solo "get", le indicamos que es de lectura.
        public String Error { get { return sError; } }
        

        public DataTable Seleccionar(int producto_id = 0)
        {
            // Para almacenar la tabla leída en memoria.
            DataTable dataTable = new DataTable();

            try
            {
                // Realizamos la conexión.
                conexionBD.Abrir();

                sqlCommand.Connection = conexionBD.Connection;

                // Indicamos el tipo de comando. En este caso un procedimiento almacenado.
                sqlCommand.CommandType = CommandType.StoredProcedure;
                
                // Sentencia a ejecutar. En este caso un procedimiento almacenado.
                sqlCommand.CommandText = "ProductosSeleccionar";
                
                // Parámetro pasado al procediento almacenado.
                sqlCommand.Parameters.AddWithValue("@producto_id", producto_id);

                // Ejecutamos la sentencia...
                sqlDataReader = sqlCommand.ExecuteReader();

                // y la guardamos en la tabla leída en la memoria.
                dataTable.Load(sqlDataReader);

                // Si me indicaron que seleccionase un único registro y este existe.
                if ((producto_id != 0) && (dataTable.Rows.Count != 0))
                {
                    // Obtenemos las filas de la tabla en memoria (En este sólo hay una única fila).
                    DataRow[] rows = dataTable.Select();

                    // Asignamos a cada propiedad del producto el valor del registro leído.
                    Producto_id = producto_id;
                    Producto = rows[0]["producto"].ToString(); 
                    Categoria_id = Convert.ToInt32(rows[0]["categoria_id"].ToString());
                    Categoria = rows[0]["categoría"].ToString();
                    Marca_id = Convert.ToInt32(rows[0]["marca_id"].ToString());
                    Marca = rows[0]["marca"].ToString();
                    Precio = Convert.ToDouble(rows[0]["precio"].ToString());
                    Codigo = Convert.ToInt32(rows[0]["código"].ToString());
                }
            }
            finally
            {
                // Limpiamos los parámetros del comando ejecutado.
                sqlCommand.Parameters.Clear();

                // Cerramos los datos leídos.
                sqlDataReader.Close();

                // Cerramos la conexión.
                conexionBD.Cerrar();
            }

            // Devolvemos la tabla almacenada en memoria.
            return dataTable;
        }

        public bool Insertar()
        {
            // Para devolver si la operación se hizo correctamente, o no.
            bool bInsertada = false;

            sError = "";

            try
            {
                // Es similar a la selección, salvo cambiando el procedimiento almacenado y 
                // añadiendo los parámetros correspondientes.
                conexionBD.Abrir();
                sqlCommand.Connection = conexionBD.Connection;

                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "ProductosInsertar";
                sqlCommand.Parameters.AddWithValue("@categoria_id", Categoria_id);
                sqlCommand.Parameters.AddWithValue("@marca_id", Marca_id);
                sqlCommand.Parameters.AddWithValue("@producto", Producto);
                sqlCommand.Parameters.AddWithValue("@precio", Precio);
                sqlCommand.Parameters.AddWithValue("@codigo", Codigo);

                // Valor devuelto por el procedimiento almacenado (En este caso la clave primaria).
                var returnParameter = sqlCommand.Parameters.Add("@producto_id", SqlDbType.Int);
                
                // Indicamos que es un valor de sólo retorno.
                returnParameter.Direction = ParameterDirection.ReturnValue;

                // Ejecutamos la sentencia, indicando que no es una consulta SELECT, y
                // aprovechamos el número de regisros que nos devuelve. En este caso debe ser 1.
                bInsertada = sqlCommand.ExecuteNonQuery() == 1;

                // Si la inserción fue correcta, obtenemos el valor de la clave primaria.
                if (bInsertada)
                    Producto_id = Convert.ToInt32(returnParameter.Value);
            }
            catch (Exception ex)
            {
                sError = "Código duplicado.\n\n" + ex.Message;

                bInsertada = false;
            }
            finally
            {
                //Limpiar parámetros. Haced esto siempre para que no se acumule en la siguiente llamada a un procedimiento almacenado.
                sqlCommand.Parameters.Clear();

                conexionBD.Cerrar();
            }

            // Devolvemos si la operación fue correcta o no.
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
                sqlCommand.CommandText = "ProductosBorrar";
                sqlCommand.Parameters.AddWithValue("@producto_id", Producto_id);

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
                sqlCommand.CommandText = "ProductosEditar";
                sqlCommand.Parameters.AddWithValue("@producto_id", Producto_id);
                sqlCommand.Parameters.AddWithValue("@categoria_id", Categoria_id);
                sqlCommand.Parameters.AddWithValue("@marca_id", Marca_id);
                sqlCommand.Parameters.AddWithValue("@producto", Producto);
                sqlCommand.Parameters.AddWithValue("@precio", Precio);
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
