using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Series
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        SqlConnection miConexionSql;
        public Window1()
        {
            InitializeComponent();
            string miConexion = ConfigurationManager.ConnectionStrings["Series.Properties.Settings.SeriesConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);
            mostrarSeries();
        }


        //Consulta SQL que muestra el titulo y la valoracion de la serie en orden descendente
        public void mostrarSeries()
        {
            string consulta = "SELECT * FROM SERIES ORDER BY Valoracion DESC";
            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta,miConexionSql);

            using(miAdaptadorSql)
            {
                DataTable datos = new DataTable();
                miAdaptadorSql.Fill(datos);
                Titulos.DisplayMemberPath = "Titulo";
                Titulos.SelectedValuePath = "Id";
                Titulos.ItemsSource = datos.DefaultView;
                Valoraciones.DisplayMemberPath = "Valoracion";
                Valoraciones.SelectedValuePath = "Id";
                Valoraciones.ItemsSource = datos.DefaultView;
            }
            
            
            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
            miConexionSql.Open();
            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();
        }


    }
}
