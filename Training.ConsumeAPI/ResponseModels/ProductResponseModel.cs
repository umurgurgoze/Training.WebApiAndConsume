using System;

namespace Training.ConsumeAPI.ResponseModels
{
    public class ProductResponseModel //Api içerisindeki product bu halde modeli dönecek.
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImagePath { get; set; }
        
    }
}
