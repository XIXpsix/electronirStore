using ElectronicsStore.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.Domain.Response
{
    public class IBaseResponse
    {
        string Description { get; set; }

        StatusCode StatusCode { get; set; }

        T? Data { get; set; }
    }
}
