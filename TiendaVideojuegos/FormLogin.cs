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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Recojo los datos que ha escrito el usuario en los textbox
            string usuarioIntroducido = txtUsuario.Text;
            string passwordIntroducida = txtPassword.Text;

            // Compruebo que no haya dejado los campos en blanco
            if (usuarioIntroducido == "" || passwordIntroducida == "")
            {
                MessageBox.Show("Por favor, rellena tanto el usuario como la contraseña.", "Hay campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salgo de la función para que no siga ejecutando
            }

            // para conectar a la base de datos
            ConexionBD miConexion = new ConexionBD();
            MySql.Data.MySqlClient.MySqlConnection conexionActiva = miConexion.ObtenerConexion();

            try
            {
                // Abro la base de datos
                miConexion.Abrir();

                // Preparo la consulta SQL con @usu y @pass en vez de concatenar texto
                string consulta = "SELECT rol FROM Usuarios WHERE nombre_usuario = @usu AND password = @pass";

                MySql.Data.MySqlClient.MySqlCommand comando = new MySql.Data.MySqlClient.MySqlCommand(consulta, conexionActiva);
                comando.Parameters.AddWithValue("@usu", usuarioIntroducido);
                comando.Parameters.AddWithValue("@pass", passwordIntroducida);

                // Ejecuto la consulta, ExecuteScalar devuelve el rol o null si no encuentra nada
                object resultado = comando.ExecuteScalar();

                if (resultado != null)
                {
                    // Si el resultado no es nulo, significa que el usuario y la contraseña coinciden en la base de datos
                    string rolUsuario = resultado.ToString();
                    MessageBox.Show("¡Bienvenido al sistema! Tu rol es: " + rolUsuario, "Login Correcto", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abro el menú principal
                    FormMenuPrincipal menu = new FormMenuPrincipal();
                    menu.Show();

                    // Oculto la ventana del login
                    this.Hide();
                }
                else
                {                    
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Si hay error con la base de datos, lo muestro
                MessageBox.Show("Error al comprobar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // me aseguro de cerrar la conexión siempre
                miConexion.Cerrar();
            }
        }
    }
}
