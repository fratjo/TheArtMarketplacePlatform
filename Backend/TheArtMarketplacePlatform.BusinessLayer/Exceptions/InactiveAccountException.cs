using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.BusinessLayer.Exceptions
{
    public class InactiveAccountException(string message) : Exception(message) { }
}