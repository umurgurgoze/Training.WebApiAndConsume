using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Training.ConsumeAPI.ResponseModels;

namespace Training.ConsumeAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("http://localhost:38671/api/products");

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ProductResponseModel>>(jsonData);
                return View(result);
            }
            else
            {
                return View(null);
            }

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductResponseModel model)
        {
            var client = _httpClientFactory.CreateClient();  //client oluştur
            var jsonData = JsonConvert.SerializeObject(model); // create tuşuna basılınca gelen modeli json çevir.
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json"); // çevirilen dosyayı utf8 düzeltmesi yap.
            var responseMessage = await client.PostAsync("http://localhost:38671/api/products", content); // api'ye içerikle birlikte gönder.
            if (responseMessage.IsSuccessStatusCode) // eğer başarılıysa index'e gönder.
            {
                return RedirectToAction("Index");
            }
            else // değilse hata mesajı ve verilerle ilgili sayfada tut
            {
                TempData["errorMessage"] = $"Bir hata ile karşılaşıldı. Hata kodu {(int)responseMessage.StatusCode}";
                return View(model);
            }
            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"http://localhost:38671/api/products/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductResponseModel>(jsonData);
                return View(data);
            }
            else
            {
                return View(null);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductResponseModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync("http://localhost:38671/api/products/{id}", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        public async Task<IActionResult> Remove(int id)
        {
            var client = _httpClientFactory.CreateClient();
            await client.DeleteAsync($"http://localhost:38671/api/products/{id}");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            /* 1 */
            var client = _httpClientFactory.CreateClient();
            /* 2 */  //Bir dosya gönderiyorsak byte dizisine çevirerek göndermeliyiz. 
            var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            /* 3 */ //Dosyayı byte dizisi haline çeviririz.Daha sonra bu içeriği formdata haline getiririz.
            var bytes = stream.ToArray();
            ByteArrayContent content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType); //content header olarak type gönderiyoruz.
            MultipartFormDataContent formData = new MultipartFormDataContent();
            formData.Add(content, "formFile", file.FileName); //api bizden formFile tipinde ve file.FileName içeriği ile birlikte bir dönüş bekler.
            /* 4 */ // ilgili formdata ile birlikte metoda post ederiz.
            await client.PostAsync("http://localhost:38671/api/products/upload", formData);
            return RedirectToAction("Index");
        }
    }
}
