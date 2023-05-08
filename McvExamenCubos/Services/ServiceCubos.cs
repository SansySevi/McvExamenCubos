using Azure.Storage.Blobs;
using McvExamenCubos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace McvExamenCubos.Services
{
    public class ServiceCubos
    {

        private BlobServiceClient client;

        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiCubos;

        public ServiceCubos(IConfiguration configuration, BlobServiceClient client)
        {
            this.UrlApiCubos =
                configuration.GetValue<string>("ApiUrls:ApiExamCubos");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");

            this.client = client;
        }


        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };

                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data =
                        await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }

            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data =
                        await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        //METODOs USUARIO
        public async Task<Usuario> GetPerfilUsuarioAsync
            (string token)
        {
            string request = "/api/usuarios/perfilusuario";
            Usuario usuario = await
                this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }

        public async Task GetRegisterUserAsync
            (string nombre, string email, string password, string imagen)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/register";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Usuario usuario = new Usuario();
                usuario.IdUsuario = 0;
                usuario.Nombre = nombre;
                usuario.Email = email;
                usuario.Password = password;
                usuario.ImagenPerfil = imagen;

                string json = JsonConvert.SerializeObject(usuario);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }



        public async Task<List<string>> GetMarcas()
        {
            string request = "/api/tienda/marcas";
            List<string> marcas = await
                this.CallApiAsync<List<string>>(request);
            return marcas;
        }

        public async Task<List<Cubo>> GetProductosAsync()
        {
            string request = "/api/tienda/productos";
            List<Cubo> cubos = await
                this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task<List<Cubo>> GetProductosMarcaAsync(string marca)
        {
            string request = "/api/tienda/productosmarca/" + marca;
            List<Cubo> cubos = await
                this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task<Cubo> FindProductoAsync(int idproducto)
        {
            string request = "/api/tienda/producto/" + idproducto;
            return await this.CallApiAsync<Cubo>(request);
        }

        public async Task<List<Pedido>> GetPedidosAsync(string token)
        {
            string request = "/api/usuarios/pedidos";
            return await this.CallApiAsync<List<Pedido>>(request, token);
        }

        public async Task CrearPedido(Cubo cubo, Usuario usuario, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/usuarios/realizarpedido";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);

                Pedido pedido = new Pedido();
                pedido.IdPedido = 0;
                pedido.Fecha = DateTime.UtcNow;
                pedido.IdCubo = cubo.IdCubo;
                pedido.IdUsuario = usuario.IdUsuario;

                string json = JsonConvert.SerializeObject(pedido);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task InsertarProducto(string nombre, string marca, string imagen, int precio)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/tienda/insertarproducto";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Cubo producto = new Cubo();
                producto.IdCubo = 0;
                producto.Nombre = nombre;
                producto.Marca = marca;
                producto.Imagen = imagen;
                producto.Precio = precio;

                string json = JsonConvert.SerializeObject(producto);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }
    }
}
