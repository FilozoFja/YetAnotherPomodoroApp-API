using YAPA.Models;

namespace YAPA.Interface
{
    public interface IJwtGeneratorService
    {
        public string JwtGenerator(UserModel user);
    }
}
