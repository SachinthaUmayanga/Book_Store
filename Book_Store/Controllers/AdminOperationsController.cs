using Book_Store.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book_Store.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    private readonly IBookRepository _booksRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IStockRepository _stoctRepository;

    public AdminOperationsController(IUserOrderRepository userOrderRepository, IBookRepository booksRepository, IGenreRepository genreRepository, IStockRepository stoctRepository)
    {
        _userOrderRepository = userOrderRepository;
        _booksRepository = booksRepository;
        _genreRepository = genreRepository;
        _stoctRepository = stoctRepository;
    }
    
    public async Task<IActionResult> AllOrders()
    {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
            // log exception here
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with id:{orderId} does not found.");
        }
        var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
        {
            return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, 
                Selected = order.OrderStatusId == orderStatus.Id };
        });
        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatusId = order.OrderStatusId,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
                {
                    return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, 
                        Selected = orderStatus.Id == data.OrderStatusId };
                });

                return View(data);
            }
            await _userOrderRepository.ChangeOrderStatus(data);
            TempData["successMessage"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            // catch exception here
            TempData["errorMessage"] = "Something went wrong";
        }
        return RedirectToAction(nameof(AllOrders), new { orderId = data.OrderId });
    }

    public async Task<IActionResult> Dashboard()
    {
        var orders = await _userOrderRepository.UserOrders(true);
        var booklist = await _booksRepository.GetBooks();
        var genreList = await _genreRepository.GetGenres();
        var stockList = await _stoctRepository.GetStocks();

        var newBooks = booklist.Where(b => b.BookCondition == "New").ToList();
        var usedBooks = booklist.Where(b => b.BookCondition == "Used").ToList();

        var ordermappedData = orders.GroupBy(o => o.CreateDate.ToString("yyyy/MM/dd")).Select(group => new DashboardOrderDetailsDTO
    {
        OrderDate = group.Key, 
        Count = group.Count() 
    }).ToList();

        var newGenreMappedData = newBooks
            .Join(stockList,
                  book => book.Id,
                  stock => stock.BookId,
                  (book, stock) => new { book, stock }) 
            .GroupBy(x => x.book.Genre.GenreName)
            .Select(group => new DashboardBookDetailsDTO
            {
                Category = group.Key,
                Count = group.Sum(x => x.stock.Quantity)
            })
            .ToList();        
        var usedGenreMappedData = usedBooks
            .Join(stockList,
                  book => book.Id,
                  stock => stock.BookId,
                  (book, stock) => new { book, stock }) 
            .GroupBy(x => x.book.Genre.GenreName)
            .Select(group => new DashboardBookDetailsDTO
            {
                Category = group.Key,
                Count = group.Sum(x => x.stock.Quantity)
            })
            .ToList();

        var dashboardDetails = new DashboardDTO
        {
            NewBooks = newGenreMappedData,
            UsedBooks = usedGenreMappedData,
            OrderDetails = ordermappedData
        };

        return View(dashboardDetails);
    }
}
