using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.DTOs;
using ProjectSWP391.Models.ServiceModel;
using ProjectSWP391.Models;

public class ShoppingCartController : Controller
{
    private readonly SWP391_V4Context context;
    private const string CartCookieKey = "CartItems";

    public ShoppingCartController(SWP391_V4Context _context)
    {
        context = _context;
    }

    public IActionResult AddToCart(int productId)
    {
        var product = context.Products.Find(productId);
        if (product == null)
        {
            return NotFound();
        }

        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;

        var cartItemsDict = GetCartItemsDictFromCookie();

        var cartItems = cartItemsDict.TryGetValue(accountId, out var cartItemsForAccountId)
            ? cartItemsForAccountId
            : new List<ShoppingCartModel>();

        var existingCartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
        if (existingCartItem != null)
        {
            existingCartItem.Quantity++;
        }
        else
        {
            cartItems.Add(new ShoppingCartModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image,
                Quantity = 1
            });
        }

        cartItemsDict[accountId] = cartItems;
        SetCartItemsDictToCookie(cartItemsDict);
        return RedirectToAction("Index");
    }


    public IActionResult Index()
    {
        var cartItems = GetCartItemsForCurrentUser();

        decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

        var viewModel = new ShoppingCartViewModel
        {
            CartItems = cartItems,
            TotalPrice = totalPrice
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult RemoveFromCart(int productId)
    {

        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;

        var cartItemsDict = GetCartItemsDictFromCookie();

        if (cartItemsDict.TryGetValue(accountId, out var cartItems))
        {
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                cartItemsDict[accountId] = cartItems;
                SetCartItemsDictToCookie(cartItemsDict);
            }
        }

        return RedirectToAction("Index");
    }

    private List<ShoppingCartModel> GetCartItemsForCurrentUser()
    {
        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;

        var cartItemsDict = GetCartItemsDictFromCookie();

        return cartItemsDict.TryGetValue(accountId, out var cartItemsForAccountId)
            ? cartItemsForAccountId
            : new List<ShoppingCartModel>();
    }

    private Dictionary<int, List<ShoppingCartModel>> GetCartItemsDictFromCookie()
    {
        var cartItemsJson = HttpContext.Request.Cookies[CartCookieKey];
        if (!string.IsNullOrEmpty(cartItemsJson))
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, List<ShoppingCartModel>>>(cartItemsJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing cart items from cookie: " + ex.Message);
            }
        }
        return new Dictionary<int, List<ShoppingCartModel>>();
    }

    private void SetCartItemsDictToCookie(Dictionary<int, List<ShoppingCartModel>> cartItemsDict)
    {
        var cartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItemsDict);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7)
        };
        HttpContext.Response.Cookies.Append(CartCookieKey, cartItemsJson, cookieOptions);
    }

    [HttpPost]
    public IActionResult RemoveSelectedItems(List<int> productIds)
    {
        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;
        var cartItemsDict = GetCartItemsDictFromCookie();

        if (cartItemsDict.TryGetValue(accountId, out var cartItems))
        {
            cartItems.RemoveAll(item => productIds.Contains(item.ProductId));
            cartItemsDict[accountId] = cartItems;
            SetCartItemsDictToCookie(cartItemsDict);
            return Json(new { success = true });
        }

        return Json(new { success = false });
    }

    [HttpPost]
    public IActionResult RemoveAllItems()
    {
        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;
        var cartItemsDict = GetCartItemsDictFromCookie();

        if (cartItemsDict.ContainsKey(accountId))
        {
            cartItemsDict.Remove(accountId);
            SetCartItemsDictToCookie(cartItemsDict);
            return Json(new { success = true });
        }

        return Json(new { success = false });
    }

    [HttpPost]
    public IActionResult UpdateCartItemQuantity(int productId, int quantity)
    {
        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;
        var cartItemsDict = GetCartItemsDictFromCookie();

        if (cartItemsDict.TryGetValue(accountId, out var cartItems))
        {
            var cartItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                cartItemsDict[accountId] = cartItems;
                SetCartItemsDictToCookie(cartItemsDict);
                return Json(new { success = true });
            }
        }

        return Json(new { success = false });
    }


    [HttpGet]
    public IActionResult Checkout()
    {
        var cartItems = GetCartItemsForCurrentUser();

        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index");
        }

        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;
        var account = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);

        if (account == null)
        {
            return RedirectToAction("Registration");
        }
        // Check quantity before creating order and order details
        foreach (var item in cartItems)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);

            if (product == null)
            {
                TempData["ErrorMessage"] = $"Product with ID '{item.ProductId}' not found.";
                return RedirectToAction("Index");
            }

            if (item.Quantity > product.Quantity)
            {
                TempData["ErrorMessage"] = $"Invalid quantity for '{item.ProductName}'. Quantity exceeds available stock. Max Quantity: {product.Quantity}";
                return RedirectToAction("Index");
            }
        }

        // Calculate the subtotal
        decimal subtotal = cartItems.Sum(item => item.Price * item.Quantity);

        // Set data to ViewBag
        ViewBag.CartItems = cartItems;
        ViewBag.Total = cartItems.Sum(item => item.Price * item.Quantity);

        return View(account);
    }

    [HttpPost]
    public IActionResult Checkout(string email, string phone, string fullName, string address, string addInfo)
    {
        var cartItems = GetCartItemsForCurrentUser();

        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index");
        }

        int accountId = (Global.CurrentUser != null) ? Global.CurrentUser.AccountId : -1;
        var account = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);

        var order = new Order
        {
            AccountId = accountId,
            OrderDate = DateTime.Now,
            Content = $"Email: ${email}, FullName: {fullName}, Phone: {phone}, Address: {address}, Description: {addInfo}",
            Account = account
        };

        context.Orders.Add(order);
        context.SaveChanges();

        foreach (var item in cartItems)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);

            if (product == null)
            {

                TempData["ErrorMessage"] = $"Product with ID '{item.ProductId}' not found.";
                return RedirectToAction("Index");
            }

            var orderDetail = new OrderDetail
            {
                ProductId = item.ProductId,
                Amount = item.Quantity,
                OrderId = order.OrderId
            };

            context.OrderDetails.Add(orderDetail);

            // Update the stock in the database
            product.Quantity -= item.Quantity;

            context.OrderDetails.Add(orderDetail);
        }

        context.SaveChanges();

        var cartItemsDict = GetCartItemsDictFromCookie();
        cartItemsDict[accountId] = new List<ShoppingCartModel>();
        SetCartItemsDictToCookie(cartItemsDict);

        return View("SuccessPayment");

    }
}
