using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DemonIntroAsync2429841
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBasePralelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecución(destinoBasePralelo, destinoBaseSecuencial);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //Parte secuencial

            var SW = new Stopwatch();
            SW.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }
            Console.WriteLine("Secuencial - duración en segundos: {0} ",
                SW.ElapsedMilliseconds / 1000.0);
            SW.Reset();

            SW.Start();
            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBasePralelo, imagen);

            });

            await Task.WhenAll(tareasEnumerable);
            Console.WriteLine("Paralelo - duración en segundos: {0}",
                SW.ElapsedMilliseconds / 1000.0);

            SW.Stop();



            pictureBox1.Visible = false;
        }
        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();
            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();
            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"canada{i}.jpg",
                        URL = "https://th.bing.com/th/id/OIP.OiYowo1ZX0T4ezBfXM1UAAHaEo?w=3840&h=2400&rs=1&pid=ImgDetMain"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"canada1 {i}.jpg",
                        URL = "https://www.dunyaatlasi.com/wp-content/uploads/2017/10/kanada-ulke-profili.jpg"

                    });

                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"toronto {i}.jpg",
                        URL = "https://th.bing.com/th/id/OIP.yVCLdzTyHr92-bXrFtZfUQHaE8?rs=1&pid=ImgDetMain"

                    });

            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }
        private void PrepararEjecución(string destinoBasePralelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBasePralelo))
            {
                Directory.CreateDirectory(destinoBasePralelo);

            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }
            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBasePralelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(5000); //Asincrono
            return "Felipe";
        }
        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso A finalizado");
        }
        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso B finalizado");
        }
        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000);//Asincrona
            Console.WriteLine("Proceso C finalizado");
        }
    }
}
