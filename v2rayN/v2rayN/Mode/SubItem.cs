
using SQLite;
using System.Windows;
namespace v2rayN.Mode
{
    [Serializable]
    public class SubItem
    {
        [PrimaryKey]
        public string id { get; set; }

        public string remarks { get; set; }

        public string url { get; set; }

        public string moreUrl { get; set; }

        public bool enabled { get; set; } = true;

        public string userAgent { get; set; } = string.Empty;

        public int sort { get; set; }

        public string? filter { get; set; }
        public long upload { get; set; }
        public long download { get; set; }
        public long total { get; set; }
        public long usage { get { return (download + upload); } set { } }
        
        public long expireDate { get; set; }
        public int remaningExpireDays { get; set; }
        public int UsedDataGB { get; set; }
        public int TotalDataGB { get; set; }
        public string? profileWebPageUrl { get; set; }

        public int profileUpdateInterval { get; set; }

        public long updateTime { get; set; }

        public string? convertTarget { get; set; }
        public Visibility sub_info_visible { get { return TotalDataGB>0?Visibility.Visible:Visibility.Collapsed;} }
        //public Visibility sub_info_visible { get { return Visibility.Collapsed; } }
        public double UploadMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(this.upload/1024/1024);
        }
        public double DownloadMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(this.download/1024 / 1024);
        }
        public double TotalMegaBytes()
        {
            return GetJustThreeDigitOfaNumber(this.total/1024 / 1024);
        }

        public double UploadGigaBytes()
        {
            return GetJustThreeDigitOfaNumber(this.upload / 1024 / 1024/1024);
        }
        public double DownloadGigaBytes()
        {
            return GetJustThreeDigitOfaNumber(this.download / 1024 / 1024 / 1024);
        }
        public int TotalDataGigaBytes()
        {
            return (int)((this.total / 1024 / 1024 / 1024));
        }
        public int UsedDataGigaBytes()
        {
            return (int)(this.download + this.upload) / 1024 / 1024 / 1024;
        }
        public double DownloadAndUploadTotalGigaBytes()
        {
            return GetJustThreeDigitOfaNumber((this.download + this.upload) / 1024 / 1024 / 1024);
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