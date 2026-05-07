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
    public partial class FormReservas : Form
    {
        public FormReservas()
        {
            InitializeComponent();
        }

        private void FormReservas_Load(object sender, EventArgs e)
        {
            // Configuro el calendario para que por defecto me marque una semana a partir de hoy
            dtpFechaLimite.Value = DateTime.Now.AddDays(7);

            // Llamo a las funciones de carga
            CargarClientesEnCombo();
            CargarProductosEnCombo();
            CargarReservas();
        }

        private void CargarClientesEnCombo()
        {
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                string consulta = "SELECT id_cliente, nombre_completo FROM Clientes";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                cmbClientes.DataSource = tablaVirtual;
                cmbClientes.DisplayMember = "nombre_completo";
                cmbClientes.ValueMember = "id_cliente";
                cmbClientes.SelectedIndex = -1;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally { 
                miConexion.Cerrar(); 
            }
        }

        private void CargarProductosEnCombo()
        {
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Solo muestro los juegos que tienen stock para poder reservarlos
                string consulta = "SELECT id_producto, nombre FROM Productos WHERE stock > 0";

                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                cmbProductos.DataSource = tablaVirtual;
                cmbProductos.DisplayMember = "nombre";
                cmbProductos.ValueMember = "id_producto";
                cmbProductos.SelectedIndex = -1;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally { 
                miConexion.Cerrar(); 
            }
        }

        private void CargarReservas()
        {
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                string consulta = @"SELECT r.id_reserva, c.nombre_completo AS 'Nombre Cliente', p.nombre AS 'Juego Reservado', r.cantidad AS 'Cant', 
                                   r.adelanto_pagado AS 'Señal (€)', r.fecha_reserva AS 'Fecha' FROM reservas r 
                            JOIN clientes c ON r.id_cliente = c.id_cliente 
                            JOIN productos p ON r.id_producto = p.id_producto 
                            WHERE r.estado = 'Activa'";

                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                MySql.Data.MySqlClient.MySqlDataAdapter adaptador = new MySql.Data.MySqlClient.MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                // Asigno los datos
                dgvReservas.DataSource = tablaVirtual;

                // Oculto el ID por código después de cargar los datos,  índice 0 porque es la primera columna de la consulta SQL
                if (dgvReservas.Columns.Count > 0)
                {
                    dgvReservas.Columns[0].Visible = false;
                }

                // Ajuste para que las columnas ocupen todo el ancho disponible
                dgvReservas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reservas: " + ex.Message);
            }

            finally { 
                miConexion.Cerrar(); 
            }
        }


        private void txtAdelanto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo dejo poner números, la tecla borrar y la coma
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // Evito quese ponga dos comas
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void btnCrearReserva_Click(object sender, EventArgs e)
        {
            if (cmbClientes.SelectedIndex == -1 || cmbProductos.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un cliente y un producto.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtAdelanto.Text == "")
            {
                MessageBox.Show("Introduce el dinero dejado como señal.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idCliente = Convert.ToInt32(cmbClientes.SelectedValue);
            int idProducto = Convert.ToInt32(cmbProductos.SelectedValue);
            decimal adelanto = Convert.ToDecimal(txtAdelanto.Text);
            int cantidad = 1; // Solo dejo reservar una unidad
            DateTime fechaReserva = DateTime.Now; // Fecha exacta de hoy
            string estado = "Activa";

            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Guardo la reserva en la base de datos
                string consulta = "INSERT INTO reservas (fecha_reserva, id_cliente, id_producto, cantidad, adelanto_pagado, estado) " +
                                  "VALUES (@fecha, @cliente, @producto, @cantidad, @adelanto, @estado)";

                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@fecha", fechaReserva);
                comando.Parameters.AddWithValue("@cliente", idCliente);
                comando.Parameters.AddWithValue("@producto", idProducto);
                comando.Parameters.AddWithValue("@cantidad", cantidad);
                comando.Parameters.AddWithValue("@adelanto", adelanto);
                comando.Parameters.AddWithValue("@estado", estado);

                comando.ExecuteNonQuery();

                // Resto el stock para que ese juego quede bloqueado y no se venda a otro
                string consultaStock = "UPDATE productos SET stock = stock - @cant WHERE id_producto = @idProd";
                MySql.Data.MySqlClient.MySqlCommand comandoStock = new MySql.Data.MySqlClient.MySqlCommand(consultaStock, conexionActiva);
                comandoStock.Parameters.AddWithValue("@cant", cantidad);
                comandoStock.Parameters.AddWithValue("@idProd", idProducto);

                comandoStock.ExecuteNonQuery();

                MessageBox.Show("Reserva creada. El stock ha sido descontado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpio y recargo
                cmbClientes.SelectedIndex = -1;
                cmbProductos.SelectedIndex = -1;
                txtAdelanto.Text = "";

                CargarReservas();
                CargarProductosEnCombo(); // Para que desaparezca si se ha quedado sin stock
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la reserva: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (dgvReservas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una reserva de la lista para cancelarla");
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Seguro que quieres cancelar esta reserva? El stock volverá al inventario.", "Confirmar", MessageBoxButtons.YesNo);

            if (confirmacion == DialogResult.Yes)
            {
                // Para obtener el ID de la reserva
                int idReserva = Convert.ToInt32(dgvReservas.SelectedRows[0].Cells[0].Value);


                ConexionBD miConexion = new ConexionBD();
                MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

                try
                {
                    miConexion.Abrir();

                    // Consulto qué producto y cantidad tenía esa reserva antes de borrarla
                    string consultaInfo = "SELECT id_producto, cantidad FROM reservas WHERE id_reserva = @idR";
                    MySql.Data.MySqlClient.MySqlCommand cmdInfo = new MySql.Data.MySqlClient.MySqlCommand(consultaInfo, conexionActiva);
                    cmdInfo.Parameters.AddWithValue("@idR", idReserva);

                    MySql.Data.MySqlClient.MySqlDataReader lector = cmdInfo.ExecuteReader();
                    int idProd = 0;
                    int cant = 0;
                    if (lector.Read())
                    {
                        idProd = lector.GetInt32("id_producto");
                        cant = lector.GetInt32("cantidad");
                    }
                    lector.Close();

                    // Actualizo el estado de la reserva a cancelada
                    string sqlUpdate = "UPDATE reservas SET estado = 'Cancelada' WHERE id_reserva = @idR";
                    MySql.Data.MySqlClient.MySqlCommand cmdUpdate = new MySql.Data.MySqlClient.MySqlCommand(sqlUpdate, conexionActiva);
                    cmdUpdate.Parameters.AddWithValue("@idR", idReserva);

                    cmdUpdate.ExecuteNonQuery();

                    // Devuelvo el stock al producto
                    string sqlStock = "UPDATE productos SET stock = stock + @cant WHERE id_producto = @idP";
                    MySql.Data.MySqlClient.MySqlCommand cmdStock = new MySql.Data.MySqlClient.MySqlCommand(sqlStock, conexionActiva);
                    cmdStock.Parameters.AddWithValue("@cant", cant);
                    cmdStock.Parameters.AddWithValue("@idP", idProd);

                    cmdStock.ExecuteNonQuery();
                    MessageBox.Show("Reserva cancelada y stock devuelto.");

                    CargarReservas();
                    CargarProductosEnCombo();
                }

                catch (Exception ex) { 
                    MessageBox.Show("Error: " + ex.Message); 
                }

                finally { 
                    miConexion.Cerrar(); 
                }

            }
        }

        private void btnCompletar_Click(object sender, EventArgs e)
        {
            if (dgvReservas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona la reserva que el cliente viene a recoger.");
                return;
            }

            int idReserva = Convert.ToInt32(dgvReservas.SelectedRows[0].Cells[0].Value);

            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Cambio el estado ya que el stock ya se restó al crear la reserva, 
                string sql = "UPDATE reservas SET estado = 'Completada' WHERE id_reserva = @idR";
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conexionActiva);
                cmd.Parameters.AddWithValue("@idR", idReserva);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Reserva completada. El producto ha sido entregado al cliente");
                CargarReservas();
            }
            catch (Exception ex) { 
                MessageBox.Show("Error: " + ex.Message); 
            }
            finally { 
                miConexion.Cerrar(); 

            }
        }
    }
}
