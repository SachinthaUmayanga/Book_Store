using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace Book_Store.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> AddUser(AddUserDTO model);
        Task<IdentityResult> UpdateUser(EditUserDTO model);
        Task<IdentityResult> DeleteUser(string userId);
        Task<IEnumerable<UserWithRolesDTO>> GetUsers();
        Task<IdentityUser?> GetUserById(string userId);
    }



    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddUser(AddUserDTO model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            // Create user with hashed password
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return result;

            // Assign role to the user
            if (!string.IsNullOrEmpty(model.Role))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    // If role assignment fails, delete the user and return the role errors
                    await _userManager.DeleteAsync(user);
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
                }
            }

            return result;
        }

        public async Task<IdentityUser?> GetUserById(string userId) => await _userManager.FindByIdAsync(userId);

        public async Task<IdentityResult> UpdateUser(EditUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            user.UserName = model.UserName;
            user.Email = model.Email;

            // Update user details
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return updateResult;

            // Update user role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!string.IsNullOrEmpty(model.Role) && !currentRoles.Contains(model.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles); // Remove old roles
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
            }

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "User not found."
                    });
                return await _userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error deleting user: {ex.Message}");

                // Return a fail result with a generic error message
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "An error occurred while deleting the user."
                });
            }
        }

        public async Task<IEnumerable<UserWithRolesDTO>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRoles = new List<UserWithRolesDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserWithRolesDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return userWithRoles;
        }
    }

}
