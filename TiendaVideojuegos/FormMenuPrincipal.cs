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
    public partial class FormMenuPrincipal : Form
    {
        // Me creo una variable global en este formulario para acordarme de quién ha entrado
        string rolDelUsuarioLogueado = "";

        // bandera para controlar el cierre de sesión y copias de seguridad
        bool estamosCerrandoSesion = false; 

        // Modifico el constructor para que pida el rol
        public FormMenuPrincipal(string rolQueVieneDelLogin)
        {
            InitializeComponent();
            rolDelUsuarioLogueado = rolQueVieneDelLogin;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cierro la aplicación
            Application.Exit();
        }

        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            estamosCerrandoSesion = true; // Levanto la bandera

            //Instancio de nuevo la pantalla de login para volver a empezar
            FormLogin login = new FormLogin();
            login.Show();

            // Cierro la ventana actual del menú
            this.Close();
        }

        private void gestiónDeProductosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Instancio el nuevo formulario de productos
            FormProductos formProd = new FormProductos();

            // Le digo que su formulario padre es este menú principal
            formProd.MdiParent = this;

            // Lo muestro en pantalla
            formProd.Show();
        }

        private void gestiónDeClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Instancio el formulario de clientes
            FormClientes formCli = new FormClientes();

            // Le digo que se abra dentro de este menú principal
            formCli.MdiParent = this;

            // Lo muestro en pantalla
            formCli.Show();
        }

        private void nuevaVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Instancio el formulario de ventas y lo meto en el contenedor principal
            FormVentas formVenta = new FormVentas();
            formVenta.MdiParent = this;
            formVenta.Show();
        }

        private void reservasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReservas formRes = new FormReservas();
            formRes.MdiParent = this;
            formRes.Show();
        }

        private void gestiónDeProveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormProveedores formProv = new FormProveedores();
            formProv.MdiParent = this;
            formProv.Show();
        }

        private void verInformesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormInformes formInf = new FormInformes();
            formInf.MdiParent = this;
            formInf.Show();
        }

        private void FormMenuPrincipal_Load(object sender, EventArgs e)
        {
            // Cuando arranca el menú compruebo el rol
            if (rolDelUsuarioLogueado == "Empleado")
            {

                // Si es un empleado oculto la opción de los informes
                informesToolStripMenuItem.Visible = false;

                // También le oculto la gestión de usuarios
                gestiónDeUsuariosToolStripMenuItem.Visible = false;

            }

            // Llamo a la alerta automática
            ComprobarStockCriticoAutomatico();
        }

        private void gestiónDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUsuarios formUsuarios = new FormUsuarios();
            formUsuarios.MdiParent = this;
            formUsuarios.Show();
        }

        private void ComprobarStockCriticoAutomatico()
        {
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                miConexion.Abrir();

                // Cuento cuántos juegos tienen 2 unidades o menos
                string consulta = "SELECT COUNT(*) FROM productos WHERE stock <= 2";
                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);

                // Executescalar devuelve solo la primera celda del resultado (el número que he contado)
                int juegosAgotandose = Convert.ToInt32(comando.ExecuteScalar());

                // Si hay al menos un juego a punto de agotarse lanzo la alerta
                if (juegosAgotandose > 0)
                {
                    // Construyo el mensaje
                    string textoAlerta = "¡AVISO DE INVENTARIO AUTOMÁTICO!";
                    textoAlerta = textoAlerta + Environment.NewLine + Environment.NewLine;
                    textoAlerta = textoAlerta + "Actualmente tienes " + juegosAgotandose.ToString() + " juegos con stock MÍNIMO (2 unidades o menos).";
                    textoAlerta = textoAlerta + Environment.NewLine + Environment.NewLine;

                    // Cambio la recomendación final según el rol
                    if (rolDelUsuarioLogueado == "Administrador")
                    {
                        textoAlerta = textoAlerta + "Revisa la pestaña de Informes para hacer un pedido a los proveedores";
                    }
                    else
                    {
                        // Si es empleado, le digo otra cosa
                        textoAlerta = textoAlerta + "Avisa al Administrador de la tienda para que reponga el inventario";
                    }

                    MessageBox.Show(textoAlerta, "Alerta de Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Como es un proceso automático de fondo, si falla por una caida de red o algo, no hacemos nada y no asustamos con un mensaje de error.
                // Simplemente no se mostrará la alerta
                MessageBox.Show("Error comprobando el stock: " + ex.Message, "Error Oculto");
            }
            finally
            {
                miConexion.Cerrar();
            }
        }

        private void FormMenuPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si la bandera está levantada, voy al login y NO hago copia
            if (estamosCerrandoSesion == true)
            {
                return; 
            }

            // Le pregunto si de verdad quiere salir
            DialogResult respuesta = MessageBox.Show("¿Seguro que quieres salir del sistema? Se realizará una copia de seguridad automática.", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta == DialogResult.No)
            {
                e.Cancel = true; // Cancelo el cierre de la ventana
                return;
            }

            // SI DICE QUE SÍ, HAGO LA COPIA DE SEGURIDAD AUTOMÁTICA
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                // Lo guardo en la carpeta mis documentos del ordenador y le pongo fecha al archivo
                string rutaMisDocumentos = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fechaFormateada = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
                string archivoDestino = rutaMisDocumentos + "\\CopiaSeguridad_GameStore_" + fechaFormateada + ".sql";

                // Abro conexión y lanzo el backup usando la librería instalada
                miConexion.Abrir();
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conexionActiva;
                        mb.ExportToFile(archivoDestino); // Esta línea es la hace todo el backup
                    }
                }

                MessageBox.Show("Copia de seguridad guardada automáticamente en tus Documentos:\n" + archivoDestino, "Copia Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cierro la aplicación entera
                Application.ExitThread();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar la copia de seguridad automática: " + ex.Message);
                Application.ExitThread();
            }
            finally
            {
                miConexion.Cerrar();
            }
        }
    }
}
