using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TiendaVideojuegos
{
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            // Nada más cargar la pantalla, llamo a mi función para rellenar la tabla
            CargarProductos();
        }

        // Creo un método propio para poder llamarlo cada vez que necesite actualizar la lista
        private void CargarProductos()
        {
            // Preparo mi conexión
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // consulta SQL para traerme todos los productos
                string consulta = "SELECT id_producto, codigo_barras, nombre, plataforma, precio_venta, stock, es_segunda_mano FROM Productos";

                // para ejecutar la consulta
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);

                // El adaptador hace de puente y el DataTable de una especie de tabla virtual en la memoria del ordenador
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();

                // Relleno la tabla virtual con los datos que trae el puente
                adaptador.Fill(tablaVirtual);

                // Le digo a la DataGridView visual que su fuente de datos es esta tabla virtual
                dgvProductos.DataSource = tablaVirtual;

                // oculto la columna del ID para que el usuario no la vea
                dgvProductos.Columns["id_producto"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            {
                string codigo = txtCodigo.Text;
                string nombre = txtNombre.Text;
                string plataforma = cmbPlataforma.Text;

                // Compruebo que los campos obligatorios no estén vacíos
                if (codigo == "" || nombre == "" || txtPrecio.Text == "")
                {
                    MessageBox.Show("Por favor, rellena al menos el código, el nombre y el precio.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Salgo de la función para no intentar guardar datos incompletos
                }

                // Convierto los textos de precio y stock a números
                // uso un try-catch por si escriben letras en lugar de números
                decimal precioVenta = 0;
                int stockProducto = 0;

                try
                {
                    precioVenta = Convert.ToDecimal(txtPrecio.Text);

                    // El stock puede estar vacio, así que lo compruebo antes de convertirlo
                    if (txtStock.Text != "")
                    {
                        stockProducto = Convert.ToInt32(txtStock.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Asegúrate de escribir numeros correctos en las casillas de precio y stock", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Compruebo si la casilla de segunda mano está marcada o no
                bool segundaMano = chkSegundaMano.Checked;


                // Me conecto a la base de datos para guardar la información
                ConexionBD miConexion = new ConexionBD();
                MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

                try
                {
                    miConexion.Abrir();

                    // Preparo la consulta SQL
                    string consulta = "INSERT INTO Productos (codigo_barras, nombre, plataforma, precio_venta, stock, es_segunda_mano) " +
                                      "VALUES (@codigo, @nombre, @plataforma, @precio, @stock, @segunda)";

                    MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                    // Relleno los parámetros con mis variables
                    comando.Parameters.AddWithValue("@codigo", codigo);
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@plataforma", plataforma);
                    comando.Parameters.AddWithValue("@precio", precioVenta);
                    comando.Parameters.AddWithValue("@stock", stockProducto);
                    comando.Parameters.AddWithValue("@segunda", segundaMano);

                    // ejecuto la consulta
                    comando.ExecuteNonQuery();

                    MessageBox.Show("Producto guardado en el inventario correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Llamo a mi función de cargar productos para que la tabla se actualice sola
                    CargarProductos();


                    // Limpio las casillas para que queden listas por si quiero introducir otro producto
                    txtCodigo.Text = "";
                    txtNombre.Text = "";
                    txtPrecio.Text = "";
                    txtStock.Text = "";
                    cmbPlataforma.SelectedIndex = -1; // para desmarcar el desplegable
                    chkSegundaMano.Checked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar el producto en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // me aseguro de cerrar la conexión
                    miConexion.Cerrar();
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Vacío todas las casillas para empezar de cero o cancelar una acción
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            cmbPlataforma.SelectedIndex = -1; // Desmarco el desplegable
            chkSegundaMano.Checked = false;   // y también la casilla
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Compruebo si se ha seleccionado alguna fila en la tabla
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Haz clic en el producto de la tabla que quieres eliminar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // me aseguro para evitar borrados por error
            DialogResult respuesta = MessageBox.Show("¿Estás seguro de que quieres eliminar este producto?", "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Si dice que sí lo borro
            if (respuesta == DialogResult.Yes)
            {
                // Obtengo el ID interno del producto de la fila que está seleccionada
                int idSeleccionado = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["id_producto"].Value);

                // me conecto para ejecutar el borrado
                ConexionBD miConexion = new ConexionBD();
                MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

                try
                {
                    miConexion.Abrir();

                    // Consulta SQL para borrar usando el ID
                    string consulta = "DELETE FROM Productos WHERE id_producto = @id";
                    MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                    comando.Parameters.AddWithValue("@id", idSeleccionado);

                    comando.ExecuteNonQuery();

                    MessageBox.Show("Producto eliminado correctamente del inventario.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Vuelvo a cargar la tabla para que el juego desaparezcade la pantalla
                    CargarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    miConexion.Cerrar();
                }
            }
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Compruebo si la tecla que ha pulsado el usuario NO es un número y TAMPOCO es la tecla de borrar
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Le digo al sistema que lo anule y no escriba nada en la pantalla
                e.Handled = true;

            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Compruebo que sea un número o la tecla de borrar o una coma
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // Compruebo si la tecla pulsada es una coma, pero ya existe otra coma en el texto de la casilla
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                // si ya hay una coma anulo esta segunda pulsación para que no se rompa el decimal
                e.Handled = true;
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Compruebo que no se haya hecho clic en la cabecera de las columnas (eso sería el índice -1)
            if (e.RowIndex >= 0)
            {

                // Guardo la fila en la que ha hecho clic en una variable para que sea más fácil leer el código
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

                // Empiezo a copiar el valor de cada celda a mis casillas de texto de la izquierda
                txtCodigo.Text = fila.Cells["codigo_barras"].Value.ToString();
                txtNombre.Text = fila.Cells["nombre"].Value.ToString();
                cmbPlataforma.Text = fila.Cells["plataforma"].Value.ToString();
                txtPrecio.Text = fila.Cells["precio_venta"].Value.ToString();
                txtStock.Text = fila.Cells["stock"].Value.ToString();

                // Para la casilla de segunda mano convierto el dato a booleano
                chkSegundaMano.Checked = Convert.ToBoolean(fila.Cells["es_segunda_mano"].Value);

            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            // me aseguro de que haya una fila seleccionada en la tabla 
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona primero en la tabla el producto que quieres modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extraigo el ID del producto seleccionado (la columna oculta)
            int idSeleccionado = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["id_producto"].Value);

            // Recojo los nuevos textos que se han podido cambiar en las casillas
            string codigo = txtCodigo.Text;
            string nombre = txtNombre.Text;
            string plataforma = cmbPlataforma.Text;

            if (codigo == "" || nombre == "" || txtPrecio.Text == "")
            {
                MessageBox.Show("El código, el nombre y el precio son obligatorios.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vuelvo a convertir el precio y el stock
            decimal precioVenta = 0;
            int stockProducto = 0;

            try
            {
                precioVenta = Convert.ToDecimal(txtPrecio.Text);
                if (txtStock.Text != "")
                {
                    stockProducto = Convert.ToInt32(txtStock.Text);
                }
            }
            catch
            {
                MessageBox.Show("Comprueba que el precio y el stock tengan un formato correcto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool segundaMano = chkSegundaMano.Checked;

            // Conecto con MySQL para enviar la modificación a la base de datos
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Le pongo el WHERE al final para que solo cambie el producto seleccionado y no todo lo de la tienda
                string consulta = "UPDATE Productos SET codigo_barras = @codigo, nombre = @nombre, plataforma = @plataforma, precio_venta = @precio, stock = @stock, es_segunda_mano = @segunda WHERE id_producto = @id";

                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                // Asigno los valores a los parámetros
                comando.Parameters.AddWithValue("@codigo", codigo);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@plataforma", plataforma);
                comando.Parameters.AddWithValue("@precio", precioVenta);
                comando.Parameters.AddWithValue("@stock", stockProducto);
                comando.Parameters.AddWithValue("@segunda", segundaMano);
                comando.Parameters.AddWithValue("@id", idSeleccionado);

                // Ejecuto el cambio
                comando.ExecuteNonQuery();

                MessageBox.Show("Producto actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresco la tabla para ver los cambios y aprovecho para limpiar las casillas
                CargarProductos();

                //Le digo al botón de Limpiar que se pulse a sí mismo
                btnLimpiar.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el producto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }
    }
    }

