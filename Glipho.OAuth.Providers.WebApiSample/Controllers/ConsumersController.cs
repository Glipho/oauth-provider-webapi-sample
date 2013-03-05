using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Glipho.OAuth.Providers.WebApiSample.Controllers
{
    using System.Net;

    public class ConsumersController : Controller
    {
        private readonly IConsumers consumers;

        public ConsumersController(IConsumers consumers)
        {
            this.consumers = consumers;
        }

        public ActionResult Index()
        {
            return View(this.consumers.List(0, 10).Select(c => new Models.Consumer(c)));
        }

        public ActionResult Details(string id)
        {
            var consumer = this.consumers.Get(id);
            if (consumer != null)
            {
                return View(new Models.Consumer(consumer));
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Consumer not found");
        }

        public ActionResult Create()
        {
            return View(new Models.Consumer());
        }

        [HttpPost]
        public ActionResult Create(Models.Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var createdConsumer = this.consumers.Create(consumer.Name, consumer.Callback);
                    return RedirectToAction("Details", new { id = createdConsumer.Key });
                }
                catch (OAuthException ex)
                {
                    ModelState.AddModelError(string.Empty, ex);
                }
            }

            return this.View(consumer);
        }
    }
}
