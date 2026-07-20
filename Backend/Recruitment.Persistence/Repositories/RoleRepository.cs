using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;
using System.Threading.Tasks;

namespace Recruitment.Persistence.Repositories
{
	public class RoleRepository : IRoleRepository
	{
		private readonly ApplicationDbContext _context;

		public RoleRepository(ApplicationDbContext context)
		{
			_context = context;
		}


		public async Task<Role?> GetByNameAsync(string name)
		{
			return await _context.Roles
				.FirstOrDefaultAsync(r =>
					r.Name.ToLower() == name.ToLower());
		}
	}
}