using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Store.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class UserManagementController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserManagementController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetUsers();
        return View(users);
    }
}
