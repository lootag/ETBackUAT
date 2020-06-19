using System;
using System.Collections.Generic;
using Models.Enums;

namespace Models
{
    public class FilingRequest
    {
        public FilingRequest(IList<int> ciks, DateTime dateStart, DateTime dateEnd, IList<Filing> filings, IList<Item> items)
        {
            CIKS = ciks;
            DateStart = dateStart;
            DateEnd = dateEnd;
            Filings = filings;
            Items = items;
        }
        public IList<int> CIKS { get; }
        public DateTime DateStart { get; }
        public DateTime DateEnd { get; }
        public IList<Filing> Filings { get; }
        public IList<Item> Items { get; }
    }
}