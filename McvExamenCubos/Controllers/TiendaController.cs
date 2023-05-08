using McvExamenCubos.Models;
using McvExamenCubos.Services;
using Microsoft.AspNetCore.Mvc;

namespace McvExamenCubos.Controllers
{
    public class TiendaController : Controller
    {
        private ServiceCubos service;
        private ServiceStorageBlobs serviceBlob;

        public TiendaController(ServiceCubos service, ServiceStorageBlobs serviceBlob)
        {
            this.service = service;
            this.serviceBlob = serviceBlob;
        }

        public async Task<IActionResult> Index(int? pagina)
        {

            int elementosPorPagina = 6; // Cantidad de elementos que deseas mostrar en cada página
            int paginaActual = pagina ?? 1; // Si no se proporciona una página, mostrar la primera por defecto

            // Obtener todos los productos
            List<Cubo> productos = await this.service.GetProductosAsync();

            // Calcular la cantidad total de páginas y asegurarse de que la página actual sea válida
            int totalElementos = productos.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalElementos / elementosPorPagina);
            paginaActual = paginaActual < 1 ? 1 : paginaActual;
            paginaActual = paginaActual > totalPaginas ? totalPaginas : paginaActual;

            // Obtener los elementos para la página actual
            int indiceInicio = (paginaActual - 1) * elementosPorPagina;
            List<Cubo> productosPaginaActual = productos.Skip(indiceInicio).Take(elementosPorPagina).ToList();

            // Obtener las imágenes de portada para los productos
            List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imagenescubos");

            // Pasar los productos, las imágenes de portada y la información de paginación a la vista
            ViewData["Productos"] = productosPaginaActual;
            ViewData["Imagenes"] = listBlobs;
            ViewData["PaginaActual"] = paginaActual;
            ViewData["TotalPaginas"] = totalPaginas;

            return View();
        }

        public async Task<IActionResult> CubosMarca(string marca)
        {
            List<Cubo> productos = await this.service.GetProductosMarcaAsync(marca);
            List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imagenescubos");

            ViewData["Imagenes"] = listBlobs;
            ViewData["Marca"] = marca;
            return View(productos);
        }

        public async Task<IActionResult> Details(int id)
        {
            Cubo producto = await this.service.FindProductoAsync(id);
            List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imagenescubos");
            ViewData["Imagenes"] = listBlobs;
            return View(producto);
        }

        public async Task<IActionResult> InsertarProducto()
        {
            List<string> marcas = await this.service.GetMarcas();
            ViewData["Marcas"] = marcas;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsertarProducto(string nombre, string marca, int precio, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceBlob.UploadBlobAsync
                ("imagenescubos", blobName, stream);

            }

            await this.service.InsertarProducto(nombre, marca, blobName, precio);

            return RedirectToAction("CubosMarca", "Tienda", new {marca = marca });
        }
    }
}
