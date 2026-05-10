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
    public partial class FormMenuPrincipal : Form
    {
        // Me creo una variable global en este formulario para acordarme de quién ha entrado
        string rolDelUsuarioLogueado = "";


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

            }
        }
    }
}
