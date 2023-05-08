using Microsoft.AspNetCore.Mvc;
using McvExamenCubos.Models;
using McvExamenCubos.Services;

namespace McvExamenCubos.ViewComponents
{
    public class MenuMarcasViewComponent : ViewComponent
    {

        private ServiceCubos service;

        public MenuMarcasViewComponent(ServiceCubos service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<string> marcas = await this.service.GetMarcas();
            return View(marcas);

        }
    }
}
