using System;
using Models.Enums;

namespace Persistence.DTO
{
    public class FilingGetDto
    {
        public FilingGetDto(long id, int cik, DateTime dateFiled, int filing, string filingReadable, int item, string itemReadable, string fileName)
        {
            Id = id;
            CIK = cik;
            DateFiled = dateFiled;
            Filing = filing;
            FilingReadable = filingReadable;
            Item = item;
            ItemReadable = itemReadable;
            FileName = fileName;
        }
        public long Id { get; }
        public int CIK { get; }
        DateTime DateFiled { get; }
        public int Filing { get; }
        public string FilingReadable { get; }
        public int Item { get; }
        public string ItemReadable { get; }
        public string FileName { get; }
    }
}