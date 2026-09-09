
namespace CommonLibrary.Core.Domain.Interfaces
{
    public interface IDateConvertor
    {
        DateTime ConvertShamsiToGregorian(string shamsiDate);
        string ConvertShamsiToGregorianDate(string shamsiDate);
        string ConvertGregorianToPersianDatetime(DateTime? dateTime);
        DateTime ConvertPersianDatetimeToGregorian(string pdatetime);
        DateTime? ConvertPersianDatetimeToGregorianOrNull(string pdatetime);
        DateTime ConvertPersianDatetimeToGregorian(DateTime Persiandatetime);
        int GetPersianYear(DateTime time);
        int GetPersianDay(DateTime time);
        int GetPersianMonth(DateTime time);
        string ConvertGregorianToPersianDate(DateTime dateTime);
        string ConvertGregorianToPersianDate(DateTime? dateTime);
        string GetPersianTime(DateTime dateTime);
        bool IsLeapYear(DateTime date);
    }
}
