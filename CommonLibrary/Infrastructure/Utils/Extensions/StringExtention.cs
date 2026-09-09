using System.Text;

namespace CommonLibrary.Infrastructure.Utils.Extensions
{
    public static class StringExtention
    {
        public static StringBuilder AppendQuery(this StringBuilder builder, string value)
        {
            return builder.Append($" {value}");
        }
        public static StringBuilder AppendAndQuery(this StringBuilder builder, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? builder.Append("") : builder.Append($" AND {value}");
        }
        public static StringBuilder AppendAndQuery(this StringBuilder builder, string condition, object value)
        {
            return string.IsNullOrWhiteSpace(condition) ? builder.Append("") :
                builder.Append($" AND {condition} ={value}");
        }
        public static StringBuilder AppendLikeQuery(this StringBuilder builder, string condition, string value)
        {
            return string.IsNullOrWhiteSpace(condition) ? builder.Append("")
                : builder.Append($" AND {condition} LIKE N'%{value}%'");
        }
        public static StringBuilder AppendORQuery(this StringBuilder builder, string value)
        {
            return builder.Append($" OR {value}");
        }
        public static string PersianToEnglish(this string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString());
            return input;
        }
        public static string NormilizeMobileNo(this string input)
        {
            if (!string.IsNullOrEmpty(input) && input.StartsWith("+98"))
                input = input.Replace("+98", "0");
            return input;
        }
        public static string[] Tokenize(this string sentence)
        {
            char[] seps = new char[]{'+','-','_','(',')','*','&','^','%','$','#','@','!','~','`','\'','"',':',';'
            , '.',',','>','<','[',']','}','{','|','\\','/','?',' '};
            return sentence.Split(seps, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] Normalize(string[] words)
        {
            var res = words.ToList();
            res.RemoveAll(a => a.Count() <= 2);
            return res.ToArray();
        }
        public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }
}
