using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Book_Store.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class UserManagementController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserRepository _userRepository;

    public UserManagementController(RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
    {
        _roleManager = roleManager;
        _userRepository = userRepository;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetUsers();
        return View(users);
    }


    [HttpGet]
    public IActionResult AddUser()
    {
        // Fetch available roles to populate the dropdown
        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList(); 
        return View(new AddUserDTO());
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(AddUserDTO model)
    {
        if(!ModelState.IsValid)
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList(); // Re-populate roles on validation failure
            return View(model);
        }

        var result = await _userRepository.AddUser(model);
        if (result.Succeeded)
        {
            TempData["successMessage"] = "User added successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
                
        ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList(); // Re-populate roles on validation failure
        return View(result);
    }


    [HttpGet]
    public async Task<IActionResult> UpdateUser(string id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user == null)
            return NotFound();

        return View(new EditUserDTO
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(EditUserDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userRepository.UpdateUser(model);
        if (result.Succeeded)
        {
            TempData["successMessage"] = "User update successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(result);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var result = await _userRepository.DeleteUser(id);
            if (result.Succeeded)
            {
                TempData["successMessage"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["errorMessage"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Error in DeleteUser action: {ex.Message}");

            // Notify the user of the error
            TempData["errorMessage"] = "An unexpected error occored. Please try again later.";
            return RedirectToAction(nameof(Index));
        }
    }
}
