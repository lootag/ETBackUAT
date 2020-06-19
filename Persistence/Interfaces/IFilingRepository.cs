using System;
using System.Collections.Generic;
using Models;
using Models.Enums;
using Persistence.DTO;
namespace Persistence.Interfaces
{
    public interface IFilingRepository
    {
        IList<FilingGetDto> GetByCIKDateFilingItem(FilingRequest filingRequest, IContext context);
    }
}