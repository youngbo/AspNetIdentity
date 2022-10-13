using System.Threading.Tasks;

namespace AspNetCoreIdentity.Services
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}