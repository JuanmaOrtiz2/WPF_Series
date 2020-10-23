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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Net;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Series
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection miConexionSql;

        public MainWindow()
        {
            InitializeComponent();
            string miConexion = ConfigurationManager.ConnectionStrings["Series.Properties.Settings.SeriesConnectionString"].ConnectionString;
            miConexionSql = new SqlConnection(miConexion);
            borrarDatos();
        
        }

        //Al comenzar la aplicación, borra todos los registros para incluir series desde 0
        //Si se quieren mantener las series buscadas bastaria con eliminar la linea borrarDatos()
        public void borrarDatos()
        {
            string consulta = "DELETE FROM SERIES";
            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);
            miConexionSql.Open();
            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();
        }

        
       

       
        //Boton que muestra por pantalla los datos de la serie buscada
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Titulo2.Visibility = Visibility.Hidden;
            nTitulo.Visibility = Visibility.Hidden;
            Año.Visibility = Visibility.Hidden;
            nAño.Visibility = Visibility.Hidden;
            Valoracion.Visibility = Visibility.Hidden;
            nValoracion.Visibility = Visibility.Hidden;
            Votos.Visibility = Visibility.Hidden;
            nVotos.Visibility = Visibility.Hidden;
            Poster.Visibility = Visibility.Hidden;

            Serie buscar = new Serie();
            buscar.buscarSerie(Titulo.Text); 
            

            //Si la serie existe, mostraremos sus datos en la pantalla
            if((buscar.response).Equals("True"))
            {

                Titulo2.Visibility = Visibility.Visible;
                nTitulo.Visibility = Visibility.Visible;
                nTitulo.Text = buscar.titulo;
                Año.Visibility = Visibility.Visible;
                nAño.Visibility = Visibility.Visible;
                nAño.Text = buscar.año;
                Valoracion.Visibility = Visibility.Visible;
                nValoracion.Visibility = Visibility.Visible;
                nValoracion.Text = buscar.valoracion;
                Votos.Visibility = Visibility.Visible;
                nVotos.Visibility = Visibility.Visible;
                nVotos.Text = buscar.votos;



                //Si no hay poster, obviamente, no se mostrará
                if(!(buscar.poster).Equals("N/A"))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(buscar.poster); ;
                    bitmapImage.EndInit();

                    Poster.Visibility = Visibility.Visible;
                    Poster.Source = bitmapImage;
                }
                
            }

            //Si el titulo buscado ya lo teniamos en la base de datos, lo eliminaremos para insertarlo de nuevo, así no tendremos series duplicadas.
            if(buscar.titulo != null)
            {
                string cons = "DELETE FROM SERIES WHERE Titulo=@t";
                SqlCommand miSqlCommand = new SqlCommand(cons, miConexionSql);
                miConexionSql.Open();
                miSqlCommand.Parameters.AddWithValue("@t", buscar.titulo);
                miSqlCommand.ExecuteNonQuery();
                miConexionSql.Close();

                string consulta = "INSERT INTO SERIES (Titulo,Valoracion) VALUES (@t,@v)";
                SqlCommand miSqlCommand2 = new SqlCommand(consulta, miConexionSql);
                miConexionSql.Open();
                miSqlCommand2.Parameters.AddWithValue("@t", buscar.titulo);
                miSqlCommand2.Parameters.AddWithValue("@v", buscar.valoracion);

                miSqlCommand2.ExecuteNonQuery();
                miConexionSql.Close();
            }

            


        }

        //Botón que nos ordena en otra pantalla las series de mayor valoración a menos
        private void BtnOrdenar_Click(object sender, RoutedEventArgs e)
        {
            Window1 ventana = new Window1();
            ventana.Owner = this;
            ventana.ShowDialog();
        }


        public class Serie
        {
            public string titulo { get; set; }
            public string año { get; set; }
            public string valoracion { get; set; }
            public string votos { get; set; }
            public string poster { get; set; }
            public string response { get; set; }






            string urlText;


            public Serie() { }


            //Método que se encarga de obtener la respuesta del recurso web.
            public void callurl(string url)
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                urlText = reader.ReadToEnd();              
            }


            //Método que obtiene un JSON, lo transforma a objeto y rellena los atributos de la clase con los datos que nos interesan
            public void buscarSerie(string s)
            {
                callurl("https://omdbapi.com/?apiKey=85f63129&t=" + s);
                dynamic resultado = JsonConvert.DeserializeObject(urlText);
                titulo = resultado.Title;
                año = resultado.Year;
                valoracion = resultado.imdbRating;
                votos = resultado.imdbVotes;
                poster = resultado.Poster;
                response = resultado.Response;               
            }

           

                    
        }

        
    }
}
