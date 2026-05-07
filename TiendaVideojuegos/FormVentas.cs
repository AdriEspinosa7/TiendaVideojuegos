using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiendaVideojuegos
{
    public partial class FormVentas : Form
    {
        public FormVentas()
        {
            InitializeComponent();
        }

        private void FormVentas_Load(object sender, EventArgs e)
        {
            // Nada más abrir la pantalla de ventas llamo a las funciones para cargar los datos en los desplegables
            CargarClientesEnCombo();
            CargarProductosEnCombo();
        }

        private void CargarClientesEnCombo()
        {
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Busco el ID y el nombre de todos los clientes
                string consulta = "SELECT id_cliente, nombre_completo FROM Clientes";
                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                MySql.Data.MySqlClient.MySqlDataAdapter adaptador = new MySql.Data.MySqlClient.MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                // Enlazo la tabla con el ComboBox de clientes
                cmbClientes.DataSource = tablaVirtual;

                // Le digo qué columna quiero que se lea en pantalla, el nombre, y qué columna me guardo yo de forma oculta (el ID)
                cmbClientes.DisplayMember = "nombre_completo";
                cmbClientes.ValueMember = "id_cliente";

                // Dejo el combo sin seleccionar nada por defecto para obligar al empleado a elegir uno
                cmbClientes.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void CargarProductosEnCombo()
        {
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Busco los productos. Importante!! solo busco los que tengan un stock mayor que 0. No puedo vender lo que no tengo :/
                string consulta = "SELECT id_producto, nombre FROM Productos WHERE stock > 0";
                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                MySql.Data.MySqlClient.MySqlDataAdapter adaptador = new MySql.Data.MySqlClient.MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                // Enlazo la tabla con el comboBox de productos
                cmbProductos.DataSource = tablaVirtual;

                // Configuro lo que se ve y lo que se oculta
                cmbProductos.DisplayMember = "nombre";
                cmbProductos.ValueMember = "id_producto";

                cmbProductos.SelectedIndex = -1;
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

        private void btnAñadirCarrito_Click(object sender, EventArgs e)
        {
            // Compruebo que haya seleccionado un cliente y un producto obligatoriamente
            if (cmbClientes.SelectedIndex == -1 || cmbProductos.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un cliente y un producto antes de añadir al carrito.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Compruebo que la cantidad sea un número válido y mayor que 0
            int cantidad = 0;
            try
            {
                cantidad = Convert.ToInt32(txtCantidad.Text);
                if (cantidad <= 0)
                {
                    MessageBox.Show("La cantidad a vender debe ser al menos 1", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            catch
            {
                MessageBox.Show("Escribe una cantidad numérica válida", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtengo los IDs de los desplegables y el nombre
            int idProducto = Convert.ToInt32(cmbProductos.SelectedValue);
            string nombreProducto = cmbProductos.Text;

            // Me conecto para consultar el precio actual del juego
            decimal precioUnitario = 0;
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();


                // Busco el precio del producto seleccionado
                string consulta = "SELECT precio_venta FROM Productos WHERE id_producto = @id";
                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@id", idProducto);

                // ExecuteScalar porque sé que la consulta me devuelve un único dato (el precio)
                precioUnitario = Convert.ToDecimal(comando.ExecuteScalar());
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el precio de la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salgo para no añadir un producto sin precio
            }

            finally
            {
                miConexion.Cerrar();
            }

            // Calculo el subtotal de esta línea
            decimal subtotal = precioUnitario * cantidad;

            // Añado la fila directamente al DataGridView del carrito
            dgvCarrito.Rows.Add(idProducto, nombreProducto, cantidad, precioUnitario, subtotal);

            // para que actualice la etiqueta del total
            CalcularTotal();
        }

        // Creo un método aparte para sumar el total y también poder usarlo  cuando elimine algo del carrito.
        private void CalcularTotal()
        {
            decimal sumaTotal = 0;

            // Recorro todas las filas que hay en el carrito
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                // Sumo el valor de la columna Subtotal (número 4 empezando a contar desde el 0)
                sumaTotal = sumaTotal + Convert.ToDecimal(fila.Cells[4].Value);
            }

            // Muestro el resultado en la etiqueta grande dándole un formato de dos decimales y el símbolo €
            lblTotal.Text = "Total: " + sumaTotal.ToString("0.00") + " €";
        
    }

        private void btnEliminarDelCarrito_Click(object sender, EventArgs e)
        {
            // Compruebo si se ha seleccionado alguna fila del carrito
            if (dgvCarrito.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona en la tabla el producto que quieres quitar del carrito.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Borro la fila seleccionada directamente
            dgvCarrito.Rows.Remove(dgvCarrito.SelectedRows[0]);

            // Vuelvo a llamar a la función para que recalcule el total con los productos que han quedado en la lista
            CalcularTotal();
        }

        private void btnFinalizarVenta_Click(object sender, EventArgs e)
        {
            // Compruebo que no se intente hacer una venta vacía o sin cliente
            if (cmbClientes.SelectedIndex == -1)
            {
                MessageBox.Show("Tienes que seleccionar un cliente para poder cobrarle", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("El carrito está vacío, añade algún juego primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extraigo los datos de la venta
            int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
            DateTime fechaActual = DateTime.Now; // Cojo la fecha y hora exacta de mi ordenador

            // Calculo el total sumando las filas para asegurarme de que es exacto
            decimal totalVenta = 0;
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                totalVenta = totalVenta + Convert.ToDecimal(fila.Cells[4].Value); // la columna 4 es el Subtotal
            }

           
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // GUARDAR LA VENTA
                string consultaVenta = "INSERT INTO Ventas (id_cliente, fecha, total) VALUES (@cliente, @fecha, @total)";
                MySql.Data.MySqlClient.MySqlCommand comandoVenta = new MySql.Data.MySqlClient.MySqlCommand(consultaVenta, conexionActiva);
                comandoVenta.Parameters.AddWithValue("@cliente", idCliente);
                comandoVenta.Parameters.AddWithValue("@fecha", fechaActual);
                comandoVenta.Parameters.AddWithValue("@total", totalVenta);
                comandoVenta.ExecuteNonQuery();

                // Recupero el ID que MySQL genera automáticamente
                long idVentaGenerado = comandoVenta.LastInsertedId;

                // GUARDAR LOS DETALLES Y RESTAR STOCK
                // Recorro el carrito fila por fila
                foreach (DataGridViewRow fila in dgvCarrito.Rows)
                {
                    int idProducto = Convert.ToInt32(fila.Cells[0].Value); // ID del producto
                    int cantidad = Convert.ToInt32(fila.Cells[2].Value);   // Cantidad
                    decimal precio = Convert.ToDecimal(fila.Cells[3].Value); // Precio Unitario

                    // Guardo la línea de detalle uniéndola al ID de la venta
                    string consultaDetalle = "INSERT INTO detalles_venta (id_venta, id_producto, cantidad, precio_unitario) VALUES (@idVenta, @idProd, @cant, @precio)";
                    MySql.Data.MySqlClient.MySqlCommand comandoDetalle = new MySql.Data.MySqlClient.MySqlCommand(consultaDetalle, conexionActiva);

                    // Añado losparámetros y despues ejecuto
                    comandoDetalle.Parameters.AddWithValue("@idVenta", idVentaGenerado);
                    comandoDetalle.Parameters.AddWithValue("@idProd", idProducto);
                    comandoDetalle.Parameters.AddWithValue("@cant", cantidad);
                    comandoDetalle.Parameters.AddWithValue("@precio", precio);
                                        
                    comandoDetalle.ExecuteNonQuery();

                    // Hago un update para restar el stock en el inventario
                    string consultaStock = "UPDATE Productos SET stock = stock - @cantRestar WHERE id_producto = @idProdRestar";
                    MySql.Data.MySqlClient.MySqlCommand comandoStock = new MySql.Data.MySqlClient.MySqlCommand(consultaStock, conexionActiva);
                    comandoStock.Parameters.AddWithValue("@cantRestar", cantidad);
                    comandoStock.Parameters.AddWithValue("@idProdRestar", idProducto);

                    comandoStock.ExecuteNonQuery();
                }

                // TODO OK
                MessageBox.Show("¡Venta finalizada con éxito! El stock del inventario se ha actualizado automáticamente.", "Venta completada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpio toda la pantalla para que el empleado pueda hacer la siguiente venta
                dgvCarrito.Rows.Clear();
                cmbClientes.SelectedIndex = -1;
                cmbProductos.SelectedIndex = -1;
                txtCantidad.Text = "1";
                lblTotal.Text = "Total: 0,00 €";

                // Vuelvo a llamar a mi función de cargar productos para que el desplegable se actualice y que si hay algún juego que se haya quedado sin stock ya no aparezca
                CargarProductosEnCombo();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error crítico al procesar la venta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                miConexion.Cerrar();
            }
        }
    }
}
