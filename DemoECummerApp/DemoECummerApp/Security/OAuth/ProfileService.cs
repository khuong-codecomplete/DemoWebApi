using DemoECommerceApp.Security.OAuth;
using DemoECummerApp.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoECummerApp.Security.OAuth
{
    public class ProfileService : IProfileService
    {
        private readonly DemoDatabaseContext _context;
        public ProfileService(DemoDatabaseContext context)
        {
            _context = context;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext profileContext)
        {
            if (!string.IsNullOrEmpty(profileContext.Subject.Identity.Name))
            {
                var customer = await _context.Customers.SingleOrDefaultAsync(m => m.Email == profileContext.Subject.Identity.Name);
                if (customer != null)
                {
                    var claims = ResourceOwnerPasswordValidator.GetUserClaims(customer);
                    profileContext.IssuedClaims = claims.Where(x =>
                    profileContext.RequestedClaimTypes.Contains(x.Type)).ToList();
                }
            }
            else
            {
                var customerId = profileContext.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
                if (!string.IsNullOrEmpty(customerId.Value))
                {
                    var customer = await _context.Customers.SingleOrDefaultAsync(u => u.Id == Guid.Parse(customerId.Value));
                    if (customer != null)
                    {
                        var claims = ResourceOwnerPasswordValidator.GetUserClaims(customer);
                        profileContext.IssuedClaims = claims.Where(x =>
                        profileContext.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var customerId = context.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Id);

                if (!string.IsNullOrEmpty(customerId.Value))
                {
                    var customer = await _context.Customers.SingleOrDefaultAsync
                        (m => m.Id == Guid.Parse(customerId.Value));

                    context.IsActive = customer != null;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
