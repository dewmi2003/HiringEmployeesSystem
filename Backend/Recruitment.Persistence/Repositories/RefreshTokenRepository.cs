using Microsoft.EntityFrameworkCore;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;


namespace Recruitment.Persistence.Repositories
{

public class RefreshTokenRepository :
IRefreshTokenRepository
{

private readonly ApplicationDbContext _context;



public RefreshTokenRepository(
ApplicationDbContext context)
{
_context=context;
}



public async Task AddAsync(
RefreshToken token)
{

await _context.RefreshTokens.AddAsync(token);

await _context.SaveChangesAsync();

}




public async Task<RefreshToken?> GetAsync(
string token)
{

return await _context.RefreshTokens
.FirstOrDefaultAsync(x=>x.Token==token);

}





public async Task RevokeAsync(
string token)
{

var item =
await GetAsync(token);


if(item != null)
{

item.RevokedAt =
DateTime.UtcNow;


await _context.SaveChangesAsync();

}

}

}

}