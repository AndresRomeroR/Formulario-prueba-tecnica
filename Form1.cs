using System.Text;
using Newtonsoft.Json;
using static Formulario_prueba_tecnica.Form1;

namespace Formulario_prueba_tecnica
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await LoadEstadoCivilAsync();
        }

        private async Task LoadEstadoCivilAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7268");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("/api/Constant");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ConstantResponse>(jsonData);

                        if (result?.Data != null && result.Data.Count >= 2)
                        {
                            radioButton1.Text = result.Data[0].Nombre; // Soltero
                            radioButton2.Text = result.Data[1].Nombre; // Casado
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error al cargar los estados civiles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Excepción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class EstadoCivil
        {
            public string Nombre { get; set; }
        }

        public class ConstantResponse
        {
            public string Message { get; set; }
            public string Exception { get; set; }
            public List<EstadoCivil> Data { get; set; }
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    MessageBox.Show("Por favor, selecciona un estado civil.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Detener el flujo si no hay selección
                }

                // Recopilar los datos del formulario
                string nombre = textBox1.Text;
                string apellido = textBox2.Text;
                string tipoDocumento = comboBox1.Text;
                DateTime fechaNacimiento = dateTimePicker1.Value;
                decimal valorAGanar = decimal.Parse(textBox3.Text);
                int estadoCivil = 0;
                if (radioButton1.Checked)
                {
                    estadoCivil = 1; 
                }
                else if (radioButton2.Checked)
                {
                    estadoCivil = 2;
                }

                // Crear el objeto para enviar
                var user = new
                {
                    nombre = nombre,
                    apellido = apellido,
                    tipoDocumento = tipoDocumento,
                    fechaNacimiento = fechaNacimiento.ToString("o"), // ISO 8601
                    valorAGanar = valorAGanar,
                    idEstadoCivil = estadoCivil
                };

                // Serializar el objeto a JSON
                string jsonData = JsonConvert.SerializeObject(user);

                // Configurar la solicitud HTTP
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7268");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Realizar el POST
                    HttpResponseMessage response = await client.PostAsync("/api/User", content);

                    // Verificar la respuesta
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Usuario registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al registrar usuario: {error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Excepción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
