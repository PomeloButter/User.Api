namespace User.Identity.Services
{
    public interface IAuthCodeService
    {
        bool Validate(string phone, string authcode);
    }
}