using McvExamenCubos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using McvExamenCubos.Filters;
using McvExamenCubos.Services;

namespace McvExamenCubos.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceCubos service;
        private ServiceStorageBlobs serviceblob;

        public UsuariosController(ServiceCubos service, ServiceStorageBlobs serviceblob)
        {
            this.service = service;
            this.serviceblob = serviceblob;
        }

        [AuthorizeUsuarios]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> Perfil()
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await
                this.service.GetPerfilUsuarioAsync(token);
            BlobModel blobPerfil = await this.serviceblob.FindBlobPerfil("imagenesperfil", usuario.ImagenPerfil, usuario.Email);
            ViewData["IMAGEN_PERFIL"]= blobPerfil;
            return View(usuario);
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> Pedidos()
        {

            string token = HttpContext.Session.GetString("TOKEN");
            List<Pedido> pedidos = await this.service.GetPedidosAsync(token);
            return View(pedidos);
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> RealizarPedido(int idCubo)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            Usuario user = await this.service.GetPerfilUsuarioAsync(token);

            Cubo producto = await this.service.FindProductoAsync(idCubo);
            await this.service.CrearPedido(producto, user, token);
            return RedirectToAction("Pedidos", "Usuarios");
        }

    }
}
