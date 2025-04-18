using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.BusinessLayer.Exceptions
{
    public class InvalidRoleException(string message) : Exception(message) { }
}