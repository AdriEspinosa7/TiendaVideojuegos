using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient; 

namespace TiendaVideojuegos
{
    public class ConexionBD
    {
        // Preparo la cadena de conexión con los datos por defecto de mi XAMPP local
        // el usuario es root y no tiene contraseña por defecto
        private string cadenaConexion = "Server=localhost; Database=tienda_videojuegos; Uid=root; Pwd=;";

        // Creo el objeto de la conexión
        private MySqlConnection conexion;

        // Método para instanciar y devolver la conexión
        public MySqlConnection ObtenerConexion()
        {
            // Instancio la conexión
            conexion = new MySqlConnection(cadenaConexion);
            return conexion;
        }

        // Método para abrir la base de datos
        public void Abrir()
        {
            try
            {
                // Compruebo que la conexión esté cerrada antes de abrirla
                if (conexion.State == System.Data.ConnectionState.Closed)
                {
                    conexion.Open();
                }
            }
            catch (Exception ex)
            {
                // Si algo falla muestro un mensaje (como por ejemplo, si se me olvida encender XAMPP...) 
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message,
                                "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para cerrar la base de datos y liberar recursos
        public void Cerrar()
        {
            if (conexion.State == System.Data.ConnectionState.Open)
            {
                conexion.Close();
            }
        }
    }
}