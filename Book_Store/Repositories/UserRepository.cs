using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace Book_Store.Repositories
{
    public interface IUserRepository
    {
        Task AddUser(IdentityUser user);
        Task UpdateUser(IdentityUser user);
        Task DeleteUser(IdentityUser user);
        //Task<IdentityUser?> GetUserById(string id);
        //Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IEnumerable<UserWithRolesViewModel>> GetUsers();
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

        public async Task AddUser(IdentityUser user)
        {
            await _userManager.CreateAsync(user);
        }

        public async Task UpdateUser(IdentityUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(IdentityUser user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        //public async Task<IdentityUser?> GetUserById(string id) => await _context.Users.FindAsync(id);

        //public async Task<IEnumerable<IdentityUser>> GetUsers() => await _context.Users.ToListAsync();

        public async Task<IEnumerable<UserWithRolesViewModel>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRoles.Add(new UserWithRolesViewModel
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

    public class UserWithRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }

}
