using Abc.Northwind.Entities.Concrete;
using Abc.Northwind.MvcWebUI.ExtensionMethods;
using Microsoft.AspNetCore.Http;

namespace Abc.Northwind.MvcWebUI.Services
{
    public class CartSessionService : ICartSessionService
    {
        // session işlemlerini controller'a direkt yazmayıp dependency injection kullanmak için business kodunu buraya yazdık
        // session sadece controller'larda geldiği için burada IHttpContextAccessor kullanarak çağırıyoruz

        private IHttpContextAccessor _httpContextAccessor;
        public CartSessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Cart GetCart()
        {
            Cart cartToCheck = _httpContextAccessor.HttpContext.Session.GetObject<Cart>("cart");
            if (cartToCheck == null)
            {
                _httpContextAccessor.HttpContext.Session.SetObject("cart", new Cart());
                cartToCheck = _httpContextAccessor.HttpContext.Session.GetObject<Cart>("cart");
            }

            return cartToCheck;
        }

        public void SetCart(Cart cart)
        {
            _httpContextAccessor.HttpContext.Session.SetObject("cart", cart);
        }
    }
}
