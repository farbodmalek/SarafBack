using CommonLibrary.Core.Domain.Interfaces;
using System.Globalization;

namespace CommonLibrary.Infrastructure.Utils
{
    public sealed class DateConvertor : IDateConvertor
    {
        private const string format = "{0}/{1}/{2} {3}:{4}:{5} ";
        private const string dateFormat = "{0}/{1}/{2}";
        private const string timeFormat = "{0}:{1}:{2}";

        public DateConvertor() { }

        public DateTime ConvertShamsiToGregorian(string shamsiDate)
        {
            PersianCalendar pc = new PersianCalendar();

            var splitedDate = shamsiDate.Split('/');
            DateTime dt = new DateTime(int.Parse(splitedDate[0]), int.Parse(splitedDate[1]), int.Parse(splitedDate[2]), pc);
            return dt;
        }

        public string ConvertShamsiToGregorianDate(string shamsiDate)
        {
            shamsiDate = shamsiDate.Replace("-", "/");
            PersianCalendar pc = new PersianCalendar();
            if (string.IsNullOrEmpty(shamsiDate)) return string.Empty;
            var splitedDate = shamsiDate.Split('/');
            DateTime dt = new DateTime(int.Parse(splitedDate[0]), int.Parse(splitedDate[1]), int.Parse(splitedDate[2]), pc);
            string month = dt.Month.ToString();
            if (dt.Month < 10)
                month = "0" + dt.Month;

            string day = dt.Day.ToString();
            if (dt.Day < 10)
                day = "0" + dt.Day;
            var date = string.Format("{0}-{1}-{2}", dt.Year, month, day);
            return date;
        }

        public string ConvertGregorianToPersianDatetime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                var persianCalander = new PersianCalendar();
                return string.Format(format, persianCalander.GetYear(dateTime.Value),
                    persianCalander.GetMonth(dateTime.Value), persianCalander.GetDayOfMonth(dateTime.Value), persianCalander.GetHour(dateTime.Value),
                    persianCalander.GetMinute(dateTime.Value), persianCalander.GetSecond(dateTime.Value));
            }
            return string.Empty;
        }

        public DateTime ConvertPersianDatetimeToGregorian(string pdatetime)
        {
            try
            {
                var persianCi = new CultureInfo("fa-IR");
                persianCi.DateTimeFormat.Calendar = new PersianCalendar();
                var persianCalander = new PersianCalendar();
                DateTime persianDatetime = Convert.ToDateTime(pdatetime, persianCi);
                PersianCalendar pCalender = new PersianCalendar();
                DateTime resultDate = pCalender.ToDateTime(persianCalander.GetYear(persianDatetime), persianCalander.GetMonth(persianDatetime), persianCalander.GetDayOfMonth(persianDatetime), persianCalander.GetHour(persianDatetime),
                    persianCalander.GetMinute(persianDatetime), persianCalander.GetSecond(persianDatetime), Convert.ToInt16(persianCalander.GetMilliseconds(persianDatetime)));
                return resultDate;
            }
            catch (Exception)
            {
                throw new Exception("تاریخ درست وارد شود.");
            }
        }


        public DateTime? ConvertPersianDatetimeToGregorianOrNull(string pdatetime)
        {
            try
            {
                if (string.IsNullOrEmpty(pdatetime))
                {
                    return null;
                }
                else
                {
                    var persianCi = new CultureInfo("fa-IR");
                    persianCi.DateTimeFormat.Calendar = new PersianCalendar();
                    var persianCalander = new PersianCalendar();
                    DateTime persianDatetime = Convert.ToDateTime(pdatetime, persianCi);
                    PersianCalendar pCalender = new PersianCalendar();
                    DateTime resultDate = pCalender.ToDateTime(persianCalander.GetYear(persianDatetime), persianCalander.GetMonth(persianDatetime), persianCalander.GetDayOfMonth(persianDatetime), persianCalander.GetHour(persianDatetime),
                        persianCalander.GetMinute(persianDatetime), persianCalander.GetSecond(persianDatetime), Convert.ToInt16(persianCalander.GetMilliseconds(persianDatetime)));
                    return resultDate;
                }

            }
            catch (Exception)
            {
                throw new Exception("تاریخ درست وارد شود.");
            }
        }



        public DateTime ConvertPersianDatetimeToGregorian(DateTime Persiandatetime)
        {
            PersianCalendar pcalender = new PersianCalendar();
            DateTime resultdate = pcalender.ToDateTime(Persiandatetime.Year, Persiandatetime.Month, Persiandatetime.Day, Persiandatetime.Hour, Persiandatetime.Minute, Persiandatetime.Second, Persiandatetime.Millisecond);
            return resultdate;
        }

        public int GetPersianYear(DateTime time)
        {
            var persianCalander = new PersianCalendar();
            return persianCalander.GetYear(time);
        }

        public int GetPersianDay(DateTime time)
        {
            var persianCalander = new PersianCalendar();
            return persianCalander.GetDayOfMonth(time);
        }

        public int GetPersianMonth(DateTime time)
        {
            var persianCalander = new PersianCalendar();
            return persianCalander.GetMonth(time);
        }


        public string ConvertGregorianToPersianDate(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return "";
            }
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", persianCalander.GetYear(dateTime.Value).ToString("0000"),
                persianCalander.GetMonth(dateTime.Value).ToString("00"), persianCalander.GetDayOfMonth(dateTime.Value).ToString("00"));
        }

        public string ConvertGregorianToPersianDate(DateTime dateTime)
        {
            var persianCalander = new PersianCalendar();

            return string.Format(dateFormat, persianCalander.GetYear(dateTime).ToString("0000"),
   persianCalander.GetMonth(dateTime).ToString("00"), persianCalander.GetDayOfMonth(dateTime).ToString("00")
      // ,persianCalander.GetHour(dateTime).ToString("00"),
      //persianCalander.GetMinute(dateTime).ToString("00"), persianCalander.GetSecond(dateTime).ToString("00")
      );
        }



        public string GetPersianTime(DateTime dateTime)
        {
            var persianCalander = new PersianCalendar();
            return string.Format(timeFormat, persianCalander.GetHour(dateTime).ToString("00"),
                persianCalander.GetMinute(dateTime).ToString("00"), persianCalander.GetSecond(dateTime).ToString("00"));
        }

        public bool IsLeapYear(DateTime date)
        {
            var persianCalander = new PersianCalendar();
            var year = this.GetPersianYear(date);
            return persianCalander.IsLeapYear(year);
        }
    }
}
