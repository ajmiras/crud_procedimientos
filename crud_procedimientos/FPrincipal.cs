using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace crud_procedimientos
{
    public partial class FPrincipal : Form
    {
        private const float milimetro = 2.83465f;

        public FPrincipal()
        {
            InitializeComponent();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Instanciamos la clase FProductosModificar para introducir los datos.
            FProductosModificar fProductosModificar = new FProductosModificar();

            // Mostramos el cuadro de diálogo.
            fProductosModificar.ShowDialog();

            // Si se ha pulsado el botón de aceptar.
            if (fProductosModificar.DialogResult == DialogResult.OK)
            {
                // Recargamos la tabla.
                Recargar();

                // Obtenemos la clave primaria del producto insertado.
                int producto_id = fProductosModificar.Producto_id;

                // Buscamos la fila del producto insertado.
                int rowIndex = dataGridView.Rows
                                .Cast<DataGridViewRow>()
                                .Where(r => r.Cells[0].Value.Equals(producto_id))
                                .First()
                                .Index;
                
                // Nos posicionamos en ella.
                dataGridView.CurrentCell = dataGridView[1, rowIndex];
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // Si tenemos registros en la tabla.
            if (dataGridView.RowCount > 0)
            {
                // Obtenemos la clave primaria del producto.
                int producto_id = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);

                // Instanciamos la clase FProductosModificar para modificar los datos.
                // Observar que le pasamos en el contructor la clave primaria.
                FProductosModificar fProductosModificar = new FProductosModificar(producto_id);

                // Mostramos el cuadro de diálogo.
                fProductosModificar.ShowDialog();

                // Si se ha pulsado el botón de aceptar.
                if (fProductosModificar.DialogResult == DialogResult.OK)
                {
                    // Recargamos la tabla.
                    Recargar();

                    // Buscamos la fila del producto editado.
                    int rowIndex = dataGridView.Rows
                                    .Cast<DataGridViewRow>()
                                    .Where(r => r.Cells[0].Value.Equals(producto_id))
                                    .First()
                                    .Index;
                
                    // Nos posicionamos en ella.
                    dataGridView.CurrentCell = dataGridView[1, rowIndex];
                }
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            // Si tenemos registros en la tabla y...
            // el usuario me confirma que realmente quiere borrar el registro.
            if ((dataGridView.RowCount > 0) &&
                (MessageBox.Show("¿Realmente quiere borrar el producto seleccionado?", 
                                 "Confirmación",
                                 MessageBoxButtons.YesNo, 
                                 MessageBoxIcon.Question) == DialogResult.Yes))
            {
                // Creamos una instancia de la clase CProductosBD.
                CProductosBD productosBD = new CProductosBD();

                // Obtenemos la clave principal del producto a borrar.
                productosBD.Producto_id = Convert.ToInt32(dataGridView.CurrentRow.Cells[0].Value);

                // Si el producto se borra correctamente.
                if (productosBD.Borrar())
                {
                    // Obtenemos la fila actual.
                    int rowIndex = dataGridView.CurrentCell.RowIndex;

                    // Recargamos y vamos a la fila actual, que corresponderá al siguiente producto.
                    Recargar(rowIndex);
                }
                else
                    // Sino se ha podido borrar, mensaje de error.
                    MessageBox.Show("Al borrar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRecargar_Click(object sender, EventArgs e)
        {
            // Miramos en qué fila nos encontramos. 
            // Si no tenemos filas, nos posicionamos en la primera (0).
            // En caso contrario, en la fila actual del DataGridView.
            // Observar la utilidad, en este caso, del operador ternario. Más limpio que utilizar un if.
            int rowIndex = (dataGridView.RowCount == 0) ? 0 : dataGridView.CurrentRow.Index;

            // Otra forma de mirar en qué fila nos encontramos sería con un if:
            //
            // int rowIndex = 0;
            // 
            // if (dataGridView.RowCount > 0)
            //     rowIndex = dataGridView.CurrentRow.Index;

            // Llamamos al procedimiento recargar y nos posicionamos en la fila actual.
            Recargar(rowIndex);
        }

        private void Recargar(int rowIndex = 0)
        {
            // Instanciamos la clase CProductosBD.
            CProductosBD productosBD = new CProductosBD();

            // Recargamos el DataGridView asociando el DataSource con los datos devueltos.
            dataGridView.DataSource = productosBD.Seleccionar();

            // Si tenemos datos...
            if (dataGridView.RowCount > 0)
            {
                // Comprobamos que la fila que nos indican no es superior a la cantidad de filas que tenemos.
                // Si es así, nos posicionamos en la última fila.
                if (rowIndex >= dataGridView.RowCount)
                    rowIndex = dataGridView.RowCount - 1;

                // Si nos indican una fila negativa, nos posicionamos en la primera.
                // Si nos indican una fila negativa, nos posicionamos en la primera.
                if (rowIndex < 0)
                    rowIndex = 0;

                // Nos posicionamos en la fila indicada.
                dataGridView.CurrentCell = dataGridView[1, rowIndex];
            }
        }

        private void FPrincipal_Load(object sender, EventArgs e)
        {
            // Cargamos la tabla de productos.
            Recargar();

            // No permitimos que nos inserten filas a través del DataGridView.
            dataGridView.AllowUserToAddRows = false;

            // Las filas de la cabecera las ponemos centradas.
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Ocultamos la columna que muestra la clave primaria "id"
            dataGridView.Columns["id"].Visible = false;

            // La columna con los precios la mostramos formateada como moneda (currency)...
            dataGridView.Columns["Precio"].DefaultCellStyle.Format = "c";

            // y la alineamos a la derecha.
            dataGridView.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Si hay algún valor null, lo mostraremos con tres guiones.
            dataGridView.DefaultCellStyle.NullValue = "---";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Si el usuarios realmente quiere salir, cerramos la app.
            if (MessageBox.Show("¿Realmente quiere salir de la App?",
                                "Confirmación",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            // Nos posicionamos en la primera fila del DataGridView.
            dataGridView.CurrentCell = dataGridView[1, 0];
        }
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            // Buscamos la fila anterior.
            int rowIndex = dataGridView.CurrentRow.Index - 1;

            // Si es negativa, es porque estábamos ya en la primera fila.
            if (rowIndex < 0)
                rowIndex = 0;

            // Nos posicionamos en la fila del DataGridView.
            dataGridView.CurrentCell = dataGridView[1, rowIndex];
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            // Buscamos la fila siguiente.
            int rowIndex = dataGridView.CurrentRow.Index + 1;

            // Si es mayor que la cantidad de filas que hay en el DataGridView, entonces nos vamos a la última fila.
            if (rowIndex >= dataGridView.RowCount)
                rowIndex = dataGridView.RowCount - 1;

            // Nos posicionamos en la fila del DataGridView.
            dataGridView.CurrentCell = dataGridView[1, rowIndex];
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            // Buscamos la última fila.
            int rowIndex = dataGridView.RowCount - 1;

            // Si no había filas en el DataGridView, entonces la fila será la primera.
            if (rowIndex < 0)
                rowIndex = 0;

            // Nos posicionamos en la fila del DataGridView.
            dataGridView.CurrentCell = dataGridView[1, rowIndex];
        }

        private void btImprimir_Click(object sender, EventArgs e)
        {
            CProductosPDF cProductosPDF = new CProductosPDF();

            cProductosPDF.Imprimir();
        }

        private void btBuscar_Click(object sender, EventArgs e)
        {
            if (cbCampos.Text == "")
            {
                MessageBox.Show("Debe introducir un campo a filtrar", "Fitlrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String sFiltro = "";
            
            if (txtBuscar.Text != "")
            {
                sFiltro = cbCampos.Text + " " + txtBuscar.Text;
            }

            try
            {
                (dataGridView.DataSource as DataTable).DefaultView.RowFilter = sFiltro;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Filtrar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
