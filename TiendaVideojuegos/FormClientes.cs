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
    public partial class FormClientes : Form
    {
        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            // Nada más abrir la pantalla relleno la tabla de clientes
            CargarClientes();
        }

        // Método para actualizar la lista cuando lo necesite
        private void CargarClientes()
        {
            // Conexión a la base de datos
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Consulta para traer los datos de los clientes
                string consulta = "SELECT id_cliente, dni, nombre_completo, telefono, email FROM Clientes";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);

                // Uso el puente y la tabla en memoria
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();

                adaptador.Fill(tablaVirtual);

                // Enlazo los datos
                dgvClientes.DataSource = tablaVirtual;

                // Oculto la columna del ID para no mostrar datos internos
                dgvClientes.Columns["id_cliente"].Visible = false;

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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Recojo los textos que se han escrito
            string dni = txtDni.Text;
            string nombre = txtNombre.Text;
            string telefono = txtTelefono.Text;
            string email = txtEmail.Text;

            // Compruebo que al menos el DNI y el nombre estén rellenos (son obligatorios)
            if (dni == "" || nombre == "")
            {
                MessageBox.Show("Rellena al menos el DNI y el Nombre.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salgo para no intentar guardar clientes a medias
            }

            // Si se ha escrito un email, que al menos tenga una arroba y un punto
            if (email != "" && (!email.Contains("@") || !email.Contains(".")))
            {
                MessageBox.Show("El email no es válido. Debe contener un '@' y un punto.", "Formato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salgo para que no se guarde el email mal
            }

            if (txtDni.Text.Length != 9)
            {
                MessageBox.Show("El DNI debe tener exactamente 9 caracteres (8 números y 1 letra)", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Preparo la consulta 
                string consulta = "INSERT INTO Clientes (dni, nombre_completo, telefono, email) VALUES (@dni, @nombre, @telefono, @email)";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);

                comando.Parameters.AddWithValue("@dni", dni);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@telefono", telefono);
                comando.Parameters.AddWithValue("@email", email);

                // Ejecuto la inserción
                comando.ExecuteNonQuery();

                MessageBox.Show("¡Cliente guardado correctamente en el sistema!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresco la tabla y vacío las casillas para el siguiente cliente
                CargarClientes();

                txtDni.Text = "";
                txtNombre.Text = "";
                txtTelefono.Text = "";
                txtEmail.Text = "";
            }
            catch (Exception ex)
            {
                // Si el DNI ya existe en la base de datos lanzará una excepción y caerá aquí
                MessageBox.Show("Ha ocurrido un error. Comprueba que el DNI no esté ya registrado en otro cliente." + ex.Message, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Compruebo que se haya hecho clic en una fila válida y no en los títulos de arriba
            if (e.RowIndex >= 0)
            {
                // Guardo la fila seleccionada
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

                // Paso los textos de las celdas a las casillas de la izquierda
                txtDni.Text = fila.Cells["dni"].Value.ToString();
                txtNombre.Text = fila.Cells["nombre_completo"].Value.ToString();
                txtTelefono.Text = fila.Cells["telefono"].Value.ToString();
                txtEmail.Text = fila.Cells["email"].Value.ToString();

            }

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Vacío todas las casillas para introducir un cliente nuevo desde cero
            txtDni.Text = "";
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            // Compruebo si hay algún cliente seleccionado
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona primero en la tabla el cliente que quieres modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extraigo el ID
            int idSeleccionado = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["id_cliente"].Value);

            // Recojo los textos de las casillas 
            string dni = txtDni.Text;
            string nombre = txtNombre.Text;
            string telefono = txtTelefono.Text;
            string email = txtEmail.Text;

            if (dni == "" || nombre == "")
            {
                MessageBox.Show("El DNI y el Nombre son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si se ha escrito un email, que al menos tenga una arroba y un punto
            if (email != "" && (!email.Contains("@") || !email.Contains(".")))
            {
                MessageBox.Show("El email no es válido. Debe contener un '@' y un punto.", "Formato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salgo para que no se guarde el email mal
            }

            // Me conecto y lanzo el update
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {

                miConexion.Abrir();

                string consulta = "UPDATE Clientes SET dni = @dni, nombre_completo = @nombre, telefono = @telefono, email = @email WHERE id_cliente = @id";
                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                comando.Parameters.AddWithValue("@dni", dni);
                comando.Parameters.AddWithValue("@nombre", nombre);
                comando.Parameters.AddWithValue("@telefono", telefono);
                comando.Parameters.AddWithValue("@email", email);
                comando.Parameters.AddWithValue("@id", idSeleccionado);

                comando.ExecuteNonQuery();

                MessageBox.Show("¡Cliente actualizado correctamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Actualizo la tabla y vacío las casillas usando el botón de limpiar
                CargarClientes();

                btnLimpiar.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Haz clic en el cliente que quieres eliminar de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pido confirmación para no borrar por error
            DialogResult respuesta = MessageBox.Show("¿Estás seguro de que quieres eliminar este cliente?", "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                int idSeleccionado = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["id_cliente"].Value);

                ConexionBD miConexion = new ConexionBD();
                MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();


                try
                {
                    miConexion.Abrir();

                    string consulta = "DELETE FROM Clientes WHERE id_cliente = @id";
                    MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                    comando.Parameters.AddWithValue("@id", idSeleccionado);

                    comando.ExecuteNonQuery();

                    MessageBox.Show("Cliente eliminado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarClientes();
                    btnLimpiar.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el cliente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // me aseguro de cerrar la conexión siempre
                    miConexion.Cerrar();
                }
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permito introducir números y la tecla de borrar
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                // Si es un número lo anulo
                e.Handled = true;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            // Busca cualquier nombre del dgv que CONTENGA lo que escribas
            (dgvClientes.DataSource as DataTable).DefaultView.RowFilter = "nombre_completo LIKE '%" + txtBuscar.Text + "%'";
        }
    }
}
