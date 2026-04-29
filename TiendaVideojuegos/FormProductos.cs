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
            // 1. Preparo mi conexión
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
    }
}
