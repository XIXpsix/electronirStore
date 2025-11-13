using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.Domain.Enum
{
    public enum StatusCode
    {
        ProductNotFound = 1,
        ProductIsExists = 2,

        // Общие статусы
        OK = 200,
        InternalServerError = 500,
    }
}
