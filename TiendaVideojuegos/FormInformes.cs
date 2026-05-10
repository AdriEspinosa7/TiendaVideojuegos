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
using Microsoft.Reporting.WinForms;

namespace TiendaVideojuegos
{
    public partial class FormInformes : Form
    {
        public FormInformes()
        {
            InitializeComponent();
        }

        private void FormInformes_Load(object sender, EventArgs e)
        {
            // pongo que busque las ventas del último mes por defecto
            dtpDesde.Value = DateTime.Now.AddMonths(-1);
            dtpHasta.Value = DateTime.Now;

            // cargo el crítico nada más abrir
            CargarStockCritico();
            this.reportViewerVentas.RefreshReport();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Guardo las fechas seleccionadas en variables
            DateTime fechaInicio = dtpDesde.Value;
            DateTime fechaFin = dtpHasta.Value;

            // Conexión
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Consulta SQL controlando las fechas
                string consulta = @"SELECT v.id_venta AS Ticket, v.fecha AS Fecha, c.nombre_completo AS Cliente, v.total AS Total 
                            FROM ventas v 
                            JOIN clientes c ON v.id_cliente = c.id_cliente 
                            WHERE DATE(v.fecha) >= DATE(@desde) AND DATE(v.fecha) <= DATE(@hasta)
                            ORDER BY v.fecha DESC";

                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@desde", fechaInicio);
                comando.Parameters.AddWithValue("@hasta", fechaFin);

                // Paso los datos a la tabla virtual temporal
                MySql.Data.MySqlClient.MySqlDataAdapter adaptador = new MySql.Data.MySqlClient.MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);


                // limpio cualquier dato viejo que tuviera el visor del informe
                reportViewerVentas.LocalReport.DataSources.Clear();

                // Creo un origen de datos para el informe
                Microsoft.Reporting.WinForms.ReportDataSource origenDatosPdf = new Microsoft.Reporting.WinForms.ReportDataSource("DataSetVentas", tablaVirtual);

                // Le paso los datos al visor
                reportViewerVentas.LocalReport.DataSources.Add(origenDatosPdf);

                // Le digo al visor que se actualice y dibuje el PDF en pantalla
                reportViewerVentas.RefreshReport();
            }
            catch (Exception ex)
            {
                // mensaje de error por si falla la conexión o el reporte
                MessageBox.Show("Ha ocurrido un error al generar el informe en PDF. Detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        //PESTAÑA 2
        private void btnRefrescarStock_Click(object sender, EventArgs e)
        {
            CargarStockCritico();
        }

        private void CargarStockCritico()
        {
            ConexionBD miConexion = new ConexionBD();
            MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Muestro solo los juegos de los que me quedan 2 unidades o menos
                string consulta = @"SELECT id_producto AS ID, nombre AS Juego, stock AS 'Unidades Restantes', precio_venta AS 'Precio Venta' 
                                    FROM productos 
                                    WHERE stock <= 2 
                                    ORDER BY stock ASC";

                MySqlCommand comando = new MySqlCommand(consulta, conexionActiva);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable tablaVirtual = new DataTable();
                adaptador.Fill(tablaVirtual);

                dgvInformeStock.DataSource = tablaVirtual;
                dgvInformeStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar stock crítico: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                miConexion.Cerrar();
            }
        }
    }

}
