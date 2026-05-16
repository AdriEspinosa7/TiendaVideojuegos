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
    public partial class FormProveedores : Form
    {
        public FormProveedores()
        {
            InitializeComponent();
        }

        private void FormProveedores_Load(object sender, EventArgs e)
        {
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Uso alias para que las columnas queden bonitas en pantalla
                string consulta = "SELECT id_proveedor, nombre_empresa AS Empresa, telefono AS Teléfono, persona_contacto AS Contacto FROM proveedores";

                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                dgvProveedores.DataSource = tablaVirtual;

                // Oculto el ID 
                if (dgvProveedores.Columns.Count > 0)
                {
                    dgvProveedores.Columns[0].Visible = false;
                }

                dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            catch (Exception ex) { 
                MessageBox.Show("Error al cargar proveedores: " + ex.Message); 
            }

            finally { 

                miConexion.Cerrar(); 

            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtEmpresa.Text == "")
            {
                MessageBox.Show("El nombre de la empresa es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                string consulta = "INSERT INTO proveedores (nombre_empresa, telefono, persona_contacto) VALUES (@empresa, @telefono, @contacto)";
                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@empresa", txtEmpresa.Text);
                comando.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                comando.Parameters.AddWithValue("@contacto", txtContacto.Text);

                comando.ExecuteNonQuery();

                MessageBox.Show("Proveedor guardado con éxito.");
                CargarProveedores();
                btnLimpiar.PerformClick();
            }

            catch (Exception ex) { 
                MessageBox.Show("Error al guardar: " + ex.Message); 

            }
            finally { 
                miConexion.Cerrar(); 
            }
        }

        // para bloquear letras en el teléfono
        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProveedores.Rows[e.RowIndex];
                txtEmpresa.Text = fila.Cells["Empresa"].Value.ToString();
                txtTelefono.Text = fila.Cells["Teléfono"].Value.ToString();
                txtContacto.Text = fila.Cells["Contacto"].Value.ToString();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un proveedor de la lista.");
                return;
            }


            int idSeleccionado = Convert.ToInt32(dgvProveedores.SelectedRows[0].Cells[0].Value);

            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                string consulta = "UPDATE proveedores SET nombre_empresa = @empresa, telefono = @telefono, persona_contacto = @contacto WHERE id_proveedor = @id";

                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@empresa", txtEmpresa.Text);
                comando.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                comando.Parameters.AddWithValue("@contacto", txtContacto.Text);
                comando.Parameters.AddWithValue("@id", idSeleccionado);

                comando.ExecuteNonQuery();

                MessageBox.Show("Proveedor actualizado.");
                CargarProveedores();
                btnLimpiar.PerformClick();

            }

            catch (Exception ex) { 
                MessageBox.Show("Error al modificar: " + ex.Message); 

            }

            finally { 
                miConexion.Cerrar(); 
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count == 0) return;

            if (MessageBox.Show("¿Seguro que quieres eliminar este proveedor?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int idSeleccionado = Convert.ToInt32(dgvProveedores.SelectedRows[0].Cells[0].Value);
                ConexionBD miConexion = new ConexionBD();
                MySqlConnection conexionActiva = miConexion.ObtenerConexion();

                try
                {
                    miConexion.Abrir();

                    string consulta = "DELETE FROM proveedores WHERE id_proveedor = @id";
                    MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                    comando.Parameters.AddWithValue("@id", idSeleccionado);

                    comando.ExecuteNonQuery();

                    MessageBox.Show("Proveedor eliminado.");

                    CargarProveedores();
                    btnLimpiar.PerformClick();
                }
                catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
                finally { miConexion.Cerrar(); }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtEmpresa.Text = "";
            txtTelefono.Text = "";
            txtContacto.Text = "";
        }

        private void txtContacto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                // Si es un número lo anulo
                e.Handled = true;
            }
        }
    }
}


