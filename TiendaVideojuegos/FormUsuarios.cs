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
    public partial class FormUsuarios : Form
    {
        public FormUsuarios()
        {
            InitializeComponent();
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();
                // OJO: Por seguridad, NUNCA saco la contraseña en el SELECT para que no se vea en la tabla
                string consulta = "SELECT id_usuario, nombre_usuario AS Usuario, rol AS Rol FROM Usuarios";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                dgvUsuarios.DataSource = tablaVirtual;

                // Oculto el ID interno
                if (dgvUsuarios.Columns.Count > 0)
                {
                    dgvUsuarios.Columns[0].Visible = false;
                }
                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los usuarios: " + ex.Message);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Compruebo que el admin no se deje nada en blanco
            if (txtUsuario.Text == "" || txtPassword.Text == "" || cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, rellena todos los datos (usuario, contraseña y rol).", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nuevoUsuario = txtUsuario.Text;
            string nuevaPassword = txtPassword.Text;
            string nuevoRol = cmbRol.SelectedItem.ToString();

            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();
                string consulta = "INSERT INTO Usuarios (nombre_usuario, password, rol) VALUES (@usu, @pass, @rol)";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@usu", nuevoUsuario);
                comando.Parameters.AddWithValue("@pass", nuevaPassword);
                comando.Parameters.AddWithValue("@rol", nuevoRol);

                comando.ExecuteNonQuery();

                MessageBox.Show("¡Nuevo usuario creado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpio las casillas y recargo la tabla
                txtUsuario.Text = "";
                txtPassword.Text = "";
                cmbRol.SelectedIndex = -1;
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Compruebo que haya seleccionado un usuario de la tabla
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un usuario de la lista para eliminarlo.");
                return;
            }

            // Pregunto por seguridad antes de borrar
            DialogResult confirmacion = MessageBox.Show("¿Estás seguro de que quieres eliminar a este usuario del sistema?", "Confirmar borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                int idSeleccionado = Convert.ToInt32(dgvUsuarios.SelectedRows[0].Cells[0].Value);

                ConexionBD miConexion = new ConexionBD();
                MySqlConnection conexionActiva = miConexion.ObtenerConexion();

                try
                {
                    miConexion.Abrir();
                    string consulta = "DELETE FROM Usuarios WHERE id_usuario = @id";
                    MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                    comando.Parameters.AddWithValue("@id", idSeleccionado);
                    comando.ExecuteNonQuery();

                    MessageBox.Show("Usuario eliminado correctamente.");
                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el usuario: " + ex.Message);
                }
                finally
                {
                    miConexion.Cerrar();
                }
            }
        }
    }
}
