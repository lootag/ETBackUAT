using Models;

namespace Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        dynamic GenerateToken(User user, string secretKey);
        HashSalt ComputeHashSalt(User user);
        byte[] ComputeHashFromSalt(byte[] salt, byte[] passwordBytes);
    }
}