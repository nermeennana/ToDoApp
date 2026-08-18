using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts___Repo_Interface
{
    public interface IDataSeeding
    {
        Task DataSeedAsync();
    }
}
