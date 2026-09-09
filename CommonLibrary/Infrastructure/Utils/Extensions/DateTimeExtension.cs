using System.Globalization;

namespace CommonLibrary.Infrastructure.Utils.Extensions
{
    public static class DateTimeExtension
    {
        public static string ConvertGregorianToPersianDate(this DateTime dateTime)
        {
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", persianCalander.GetYear(dateTime).ToString("0000"),
                persianCalander.GetMonth(dateTime).ToString("00"), persianCalander.GetDayOfMonth(dateTime).ToString("00"));
        }
        public static string ConvertNullableGregorianToPersianDate(this DateTime? dateTime)
        {
            if (dateTime == null)
                return "";
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", persianCalander.GetYear(dateTime.Value).ToString("0000"),
                persianCalander.GetMonth(dateTime.Value).ToString("00"), persianCalander.GetDayOfMonth(dateTime.Value).ToString("00"));
        }
        public static string ConvertGregorianToPersianDateTime(this DateTime dateTime)
        {
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2} {3}:{4}", persianCalander.GetYear(dateTime).ToString("0000"),
                persianCalander.GetMonth(dateTime).ToString("00"), persianCalander.GetDayOfMonth(dateTime).ToString("00")
                , persianCalander.GetHour(dateTime).ToString("00"), persianCalander.GetMinute(dateTime).ToString("00"));
        }
        public static string ConvertGregorianToPersianFullDateTime(this DateTime dateTime)
        {
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2} {3}:{4}:{5}", persianCalander.GetYear(dateTime).ToString("0000"),
                persianCalander.GetMonth(dateTime).ToString("00"), persianCalander.GetDayOfMonth(dateTime).ToString("00")
                , persianCalander.GetHour(dateTime).ToString("00"), persianCalander.GetMinute(dateTime).ToString("00")
                ,persianCalander.GetSecond(dateTime).ToString("00")
                );
        }

        public static string ConvertNullableGregorianToPersianDateTime(this DateTime? dateTime)
        {
            if (dateTime == null)
                return "";
            var persianCalander = new PersianCalendar();
            return string.Format("{0}/{1}/{2} {3}:{4}", persianCalander.GetYear(dateTime.Value).ToString("0000"),
                persianCalander.GetMonth(dateTime.Value).ToString("00"), persianCalander.GetDayOfMonth(dateTime.Value).ToString("00")
                , persianCalander.GetHour(dateTime.Value).ToString("00"), persianCalander.GetMinute(dateTime.Value).ToString("00"));
        }

        public static DateTime? ConvertShamsiToGregorian(this PersianCalendar pc, string shamsiDate)
        {
            if (!string.IsNullOrEmpty(shamsiDate))
            {
                var shamsiArr = shamsiDate.Split(' ');
                var splitedDate = shamsiArr[0].Split('/');
                DateTime dt;
                if (shamsiArr.Length == 2)
                {
                    var splitedTime = shamsiArr[1].Split(':');
                    dt = new DateTime(int.Parse(splitedDate[0]), int.Parse(splitedDate[1]), int.Parse(splitedDate[2]),
                      int.Parse(splitedTime[0]), int.Parse(splitedTime[1]), 0, pc);
                }
                else
                {
                    dt = new DateTime(int.Parse(splitedDate[0]), int.Parse(splitedDate[1]), int.Parse(splitedDate[2]), pc);
                }

                return dt;
            }

            return null;
        }

        public static string ConvertGregorianToPersianDatetime(this PersianCalendar pc, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                string format = "{0}/{1}/{2} {3}:{4}";
                return string.Format(format, pc.GetYear(dateTime.Value).ToString("0000"),
                      pc.GetMonth(dateTime.Value).ToString("00"), pc.GetDayOfMonth(dateTime.Value).ToString("00")
                      , pc.GetHour(dateTime.Value).ToString("00"),
                     pc.GetMinute(dateTime.Value).ToString("00")
                     );
            }
            return "";
        }
        public static string MakeYearName(DateTime date, int month)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var year = month <= 12 ? pc.GetYear(date) : pc.GetYear(date) + 1;
            return year.ToString();
        }
        public static string GetMonthName(this DateTime date, int month)
        {
            var monthNum = monthNumValidate(month);
            return string.Format("{0} {1}", MakeMonthName(monthNum), MakeYearName(date, month));
        }
        public static string GetMonthName(this DateTime date, int month, int day)
        {
            var monthNum = monthNumValidate(month);
            return string.Format("{0} {1} {2}", day, MakeMonthName(monthNum), makeYearName(date, month));
        }
        private static string makeYearName(DateTime date, int month)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var year = month <= 12 ? pc.GetYear(date) : pc.GetYear(date) + 1;
            return year.ToString();
        }
        public static string ComputePaymentMount(this DateTime paymentDate, int mountDeadline)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var day = pc.GetDayOfMonth(paymentDate);
            var month = day <= mountDeadline ? pc.GetMonth(paymentDate.AddMonths(-1)) : pc.GetMonth(paymentDate);
            var year = pc.GetYear(paymentDate);

            string strmonth = MakeMonthName(month);
            strmonth = string.Format("{0} {1}", strmonth, year);
            return strmonth;
        }
        public static string MakeMonthName(int month)
        {
            string strmonth =
               month == 1 ? "فروردین" :
               month == 2 ? "اردیبهشت" :
               month == 3 ? "خرداد" :
               month == 4 ? "تیر" :
               month == 5 ? "مرداد" :
               month == 6 ? "شهریور" :
               month == 7 ? "مهر" :
               month == 8 ? "آبان" :
               month == 9 ? "آذر" :
               month == 10 ? "دی" :
               month == 11 ? "بهمن" :
               month == 12 ? "اسفند" :
               "";
            return strmonth;
        }
        private static int monthNumValidate(int month)
        {
            return month > 12 ? 1 : month;
        }
        public static int ComputePaymentMountNumber(this DateTime paymentDate, int mountDeadline)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var day = pc.GetDayOfMonth(paymentDate);
            var month = pc.GetMonth(paymentDate);
            var finalMonth = day <= mountDeadline ? month - 1 : month;
            finalMonth = finalMonth == 0 ? 12 : finalMonth;
            return finalMonth;
        }
        public static DateTime ToFirstHour(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }
        public static DateTime ToLastHour(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }
    }
}
