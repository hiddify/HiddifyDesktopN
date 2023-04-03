using ByteSizeLib;
using SQLite;

namespace v2rayN.Mode
{
    [Serializable]
    public class SubItem
    {
        [PrimaryKey]
        public string id
        {
            get; set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string remarks
        {
            get; set;
        }

        /// <summary>
        /// url
        /// </summary>
        public string url
        {
            get; set;
        }

        /// <summary>
        /// enable
        /// </summary>
        public bool enabled { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public string userAgent
        {
            get; set;
        } = string.Empty;


        public int sort
        {
            get; set;
        }
        public string filter { get; set; }
        public long upload { get; set; }
        public long download { get; set; }
        public long total { get; set; }
        public long expireDate { get; set; }
        public int remaningExpireDays { get; set; }
        public int UsedDataGB { get; set; }
        public int TotalDataGB { get; set; }
        public string? profileWebPageUrl { get; set; }

        public double UploadMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.upload).MegaBytes);
        }
        public double DownloadMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.download).MegaBytes);
        }
        public double TotalMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.total).MegaBytes);
        }

        public double UploadGigaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.upload).GigaBytes);
        }
        public double DownloadGigaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.download).GigaBytes);
        }
        public int TotalDataGigaBytes()
        {
            return Convert.ToInt32(Math.Floor(ByteSize.FromBytes(this.total).GigaBytes));
        }
        public int UsedDataGigaBytes()
        {
            return (int)ByteSize.FromBytes(this.download + this.upload).GigaBytes;
        }
        public double DownloadAndUploadTotalGigaBytes()
        {
            return GetJustThreeDigitOfaNumber(ByteSize.FromBits(this.download + this.upload).GigaBytes);
        }
        public DateTime ExpireToDate()
        {
            return Utils.EpochToDate(this.expireDate);
        }

        public int DaysLeftToExpire()
        {
            return this.ExpireToDate().Subtract(DateTime.Now).Days;
        }

        private double GetJustThreeDigitOfaNumber(double num)
        {
            string strNum = "";
            int counter = 0;
            foreach (var n in num.ToString().ToCharArray())
            {
                if (counter == 3)
                {
                    break;
                }
                strNum += n;
                counter++;
            }

            return double.Parse(strNum);
        }
    }
}
