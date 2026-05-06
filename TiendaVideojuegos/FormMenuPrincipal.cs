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
        public FormMenuPrincipal()
        {
            InitializeComponent();
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
    }
}
