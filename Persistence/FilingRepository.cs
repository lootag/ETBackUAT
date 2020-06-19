using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using Models.Enums;
using Persistence.Interfaces;
using Persistence.DTO;
using System.Linq;

namespace Persistence
{
    public class FilingRepository : IFilingRepository
    {
        public  IList<FilingGetDto> GetByCIKDateFilingItem(FilingRequest filingRequest, IContext context)
        {
            IList<FilingGetDto> toReturn = new List<FilingGetDto>();
            const string sql = "select * from dbo.Filings filings where CIK in @CIKS and (DateFiled BETWEEN @DateStart and @DateEnd) and Filing in @Filings and Item in @Items";
            toReturn = context.Query<FilingGetDto>(sql, new
            {
                CIKS = filingRequest.CIKS.ToArray(),
                DateStart = filingRequest.DateStart,
                DateEnd = filingRequest.DateEnd,
                Filings = filingRequest.Filings.ToArray(),
                Items = filingRequest.Items.ToArray()
            });
            return toReturn;
        }

    }
}