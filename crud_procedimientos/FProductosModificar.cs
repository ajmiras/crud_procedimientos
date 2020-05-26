using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace crud_procedimientos
{
    public partial class FProductosModificar : Form
    {
        public int Producto_id { get; set; }

        public FProductosModificar(int producto_id = 0)
        {
            InitializeComponent();

            // Clave primaria del producto indicado.
            Producto_id = producto_id;
        }

        private void FProductosModificar_Load(object sender, EventArgs e)
        {
            // Instanciamos las clases CCategoriasBD y CMarcasBD.
            CCategoriasBD categoriasBD = new CCategoriasBD();
            CMarcasBD marcasBD = new CMarcasBD();

            // Obtenemos todos los registros de la tabla.
            cbCategorias.DataSource = categoriasBD.Seleccionar();

            // Mostramos el valor del campo categoría.
            cbCategorias.DisplayMember = "categoria";

            // Indicamos que le valor seleccionado es la clave primaria.
            cbCategorias.ValueMember = "categoria_id";

            // Para las marcas hacemos lo mismo que para las categorías.
            cbMarcas.DataSource = marcasBD.Seleccionar();
            cbMarcas.DisplayMember = "marca";
            cbMarcas.ValueMember = "marca_id";

            // Si me indican un producto en concreto, es que queremos modificarlo.
            if (Producto_id != 0)
            {
                // Instanciamos la clase CProductosBD.
                CProductosBD productosBD = new CProductosBD();

                // Buscamos el producto.
                productosBD.Seleccionar(Producto_id);

                // Mostramos la clave primaria.
                txtId.Text = Convert.ToString(productosBD.Producto_id);

                // El código del producto.
                txtCodigo.Value = productosBD.Codigo;

                // El nombre del producto.
                txtProducto.Text = productosBD.Producto;

                // Buscamos en el ComboBox el índide de la categoría seleccionada.
                cbCategorias.SelectedIndex = cbCategorias.FindStringExact(productosBD.Categoria);
                // cbCategorias.SelectedValue = productosBD.Categoria_id;

                // Otra forma de asignar el índice.
                // cbMarcas.SelectedIndex = cbMarcas.FindStringExact(productosBD.Marca);
                cbMarcas.SelectedValue = productosBD.Marca_id;

                // Y finalmente, el precio.
                txtPrecio.Value = Convert.ToDecimal(productosBD.Precio);

                // Indicamos que estamos modificando.
                Text = "Productos :: Modificar";
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Verificamos que todo es correcto antes de proseguir.
            if (!Correcto())
                return;

            // Por defecto, indicamos que se pulsa el botón OK.
            DialogResult = DialogResult.OK;

            // Instanciamos la clase CProductodBD.
            CProductosBD productosBD = new CProductosBD();

            // Le pasamos a cada una de las propiedades los valores correspondientes.
            productosBD.Producto = txtProducto.Text;
            productosBD.Categoria_id = (int)cbCategorias.SelectedValue;
            productosBD.Marca_id = (int)cbMarcas.SelectedValue;
            productosBD.Precio = Convert.ToDouble(txtPrecio.Value);
            productosBD.Codigo = Convert.ToInt32(txtCodigo.Value);

            // Si estamos insertando...
            if (Producto_id == 0)
            {
                // Insertamos y verificamos que todo ha ido bien.
                if (productosBD.Insertar())
                {
                    Producto_id = productosBD.Producto_id;
                }
                else
                { 
                    MessageBox.Show("Al insertar el producto.\n" + productosBD.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Si no se ha podido insertar, devolvemos Cancel. 
                    DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                // y sino, estamos modificando.

                // Indicamos el producto a modificar.
                productosBD.Producto_id = Producto_id;

                // Verificamos que si ha habido un error.
                if (!productosBD.Editar())
                { 
                    MessageBox.Show("Al modificar el producto.\n" + productosBD.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Si no se ha podido modificar, devolvemos Cancel. 
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private bool Correcto()
        {
            if (txtProducto.Text == "")
            {
                MessageBox.Show("Debe indicar el nombre del producto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProducto.Focus();

                return false;
            }

            if (txtCodigo.Value == 0)
            {
                MessageBox.Show("Debe indicar el código del producto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProducto.Focus();

                return false;
            }

            return true;
        }

        private void Cancelar_Click(object sender, EventArgs e)
        {

        }
    }
}