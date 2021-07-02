using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.jwt.Model
{
    public interface IAuthContainerModel
    {
        string SecretKey { get; }

        string SecretAlgorithm { get; }

        int ExpireMinutes { get; }

        Claim[] Claims { get; set; }
    }
}
