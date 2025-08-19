using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Reflection;
using System.ComponentModel;


namespace Library
{
    public static class Utils
    {
        public enum quoteType { qtSingle, qtDouble }

        public static int strToIntDef(this string str, int dfltValue)
        {
            int retval;

            if ((str == null) || (str == ""))
            {
                retval = dfltValue;
            }
            else
            {

                try
                {
                    retval = Convert.ToInt32(str);
                }
                catch
                {
                    retval = dfltValue;
                }
            }
            return retval;
        }

        public static DateTime strToDateTimeDef(this string str, DateTime dfltValue)
        {
            DateTime retval = dfltValue;

            if ((str == null) || (str == ""))
            {
                retval = dfltValue;
            }
            else
            {

                try
                {
                    retval = Convert.ToDateTime(str);
                }
                catch
                {
                    retval = dfltValue;
                }
            }
            return retval;
        }

        public static double strToFloatDef(this string str, double dfltValue)
        {
            double retval = dfltValue;

            if ((str == null) || (str == ""))
            {
                retval = dfltValue;
            }
            else
            {

                try
                {
                    retval = Convert.ToDouble(str);
                }
                catch
                {
                    retval = dfltValue;
                }
            }
            return retval;
        }

        public static decimal strToDecDef(this string str, decimal dfltValue)
        {
            decimal retval = dfltValue;

            if ((str == null) || (str == ""))
            {
                retval = dfltValue;
            }
            else
            {

                try
                {
                    retval = Convert.ToDecimal(str);
                }
                catch
                {
                    retval = dfltValue;
                }
            }
            return retval;
        }

        public static String quotedString(this Guid str)
        {
            return "'" + convertSafeString(str.ToString()) + "'";
        }

        public static string quotedString(this string str)
        {
            return "'" + convertSafeString(str) + "'";
        }

        public static string quotedNullString(this string str)
        {
            if ((str == null) || (str == "")) return "NULL"; else return "'" + str.convertSafeString() + "'";
        }

        public static string nullIdStr(this int val)
        {
            if (val == 0) return "NULL"; else return val.ToString();
        }

        public static string nullIdStr(this Nullable<int> val)
        {
            if (val == null) return "NULL"; else return val.Value.ToString();
        }

        public static string quotedNullDateTime(this DateTime dt, quoteType qt)
        {
            string retval = "";
            if (dt.ToOADate() == 0)
            {
                retval = "NULL";
            }
            else
            {
                if (qt == quoteType.qtSingle) retval = "'" + dt.ToString() + "'"; else retval = "\"" + dt.ToString() + "\"";
            }

            return retval;
        }

        public static string quotedNullDateTime(this Nullable<DateTime> dt, quoteType qt)
        {
            string retval = "";
            if ((dt == null) || (dt.Value.ToOADate() == 0))
            {
                retval = "NULL";
            }
            else
            {
                if (qt == quoteType.qtSingle) retval = "'" + dt.Value.ToString() + "'"; else retval = "\"" + dt.Value.ToString() + "\"";
            }

            return retval;
        }
            
        public static string convertSafeString(this string str)
        {
            int qpos = str.IndexOf('\'');
            int npos = str.IndexOf('\0');
            if ((qpos == -1) && (npos == -1)) return str;
            string rslt = "";
            char[] chars = str.ToCharArray();
            foreach (char letter in chars)
            {
                if (letter == '\'') rslt += "''";
                else
                    if (letter != '\0') rslt += letter;
            }
            return rslt;
        }

        public static string toOption<T>(this T enumValue)
        {
            String rslt = "";
            foreach (T enumVal in Enum.GetValues(typeof(T)))
            {
                //String descr = GetEnumDescription(enumVal);
                rslt += "<option value=\"" + enumVal + "\">" + "</option>";
            }

            return rslt;
        }

        /*
        public static string GetEnumDescription(Enum value) 
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        
        public static string toDescription<TEnum>(this TEnum EnumValue) where TEnum : struct
        {
            //return GetEnumDescription((Enum)(object)((TEnum)(object)EnumValue));  
        }
         */


        public static Dictionary<string, string> jsonParse(this string rawjson)
        {
            Dictionary<string, string> outdict = new Dictionary<string, string>();
            StringBuilder keybufferbuilder = new StringBuilder();
            StringBuilder valuebufferbuilder = new StringBuilder();
            StringReader bufferreader = new StringReader(rawjson);

            int s = 0;
            bool reading = false;
            bool inside_string = false;
            bool reading_value = false;
            //break at end (returns -1)
            while (s >= 0)
            {
                s = bufferreader.Read();
                //opening of json
                if (!reading)
                {
                    if ((char)s == '{' && !inside_string && !reading) reading = true;
                    continue;
                }
                else
                {
                    //if we find a quote and we are not yet inside a string, advance and get inside
                    if (!inside_string)
                    {
                        //read past the quote
                        if ((char)s == '\'') inside_string = true;
                        continue;
                    }
                    if (inside_string)
                    {
                        //if we reached the end of the string
                        if ((char)s == '\'')
                        {
                            inside_string = false;
                            s = bufferreader.Read(); //advance pointer
                            if ((char)s == ':')
                            {
                                reading_value = true;
                                continue;
                            }
                            if (reading_value && (char)s == ',')
                            {
                                //we know we just ended the line, so put itin our dictionary
                                if (!outdict.ContainsKey(keybufferbuilder.ToString())) outdict.Add(keybufferbuilder.ToString(), valuebufferbuilder.ToString());
                                //and clear the buffers
                                keybufferbuilder.Clear();
                                valuebufferbuilder.Clear();
                                reading_value = false;
                            }
                            if (reading_value && (char)s == '}')
                            {
                                //we know we just ended the line, so put itin our dictionary
                                if (!outdict.ContainsKey(keybufferbuilder.ToString())) outdict.Add(keybufferbuilder.ToString(), valuebufferbuilder.ToString());
                                //and clear the buffers
                                keybufferbuilder.Clear();
                                valuebufferbuilder.Clear();
                                reading_value = false;
                                reading = false;
                                break;
                            }
                        }
                        else
                        {
                            if (reading_value)
                            {
                                valuebufferbuilder.Append((char)s);
                                continue;
                            }
                            else
                            {
                                keybufferbuilder.Append((char)s);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        switch ((char)s)
                        {
                            case ':':
                                reading_value = true;
                                break;
                            default:
                                if (reading_value)
                                {
                                    valuebufferbuilder.Append((char)s);
                                }
                                else
                                {
                                    keybufferbuilder.Append((char)s);
                                }
                                break;
                        }
                    }
                }
            }
            return outdict;
        }

        

        public static int numYears(DateTime date1, DateTime date2)
        {
            TimeSpan span = date1 - date2;
            Double years = span.Days / 365.25;
            if (years < 1)
                years = 1;

            return (int)Math.Ceiling(years);
        }

        /*
        private static int gcd(int a, int b)
        {
            if (b == 0)
                return a;
            else
                return gcd(b, a % b);
        }


        public static String decimalToFraction(Decimal decimalNum)
        {
            String topStr = decimalNum.ToString().Replace(Math.Truncate(decimalNum) + ".", "");

            int bot = (int)Math.Pow(10, topStr.Length);
            int top = topStr.strToIntDef(0);
            if (decimalNum > 1)
            {
                top = +top + (int)(Math.Floor(decimalNum) * bot);
            }

            int x = gcd(top, bot);

            return string.Format("{0}/{1}", top / x, bot / x);
        }
        */

        public static String decimalToFraction(Decimal decimalNum)
        {
            Decimal epsilon = (Decimal)0.0001;
            int maxIterations = 20;

            List<Decimal> d = new List<Decimal>{ 0, 1 };
            Decimal z = decimalNum;
            Decimal n = 1;
            int t = 1;

            Decimal wholeNumberPart = Math.Floor(decimalNum);
            Decimal decimalNumberPart = decimalNum - wholeNumberPart;
            while ((t < maxIterations) && (d[t] > 0) && (Math.Abs(n / d[t] - decimalNum) > epsilon))
            {
                t++;
                Decimal tmp = Math.Floor(z);
                z = z - Math.Floor(z);
                if (z > 0)
                    z = 1 / z;
                d.Add(d[t - 1] * Math.Floor(z) + d[t - 2]);
                n = Math.Floor(decimalNumberPart * d[t] + (Decimal)0.5);
            }

            if (wholeNumberPart > 0)
            {
                if (n > 0)
                    n = (d[t] * wholeNumberPart) + n;
                else
                    n = wholeNumberPart;
            }

            Decimal denom = d[t];
            if (denom <= 0)
                denom = 1;

            return string.Format("{0}/{1}", n, denom);
        }



        static String cryptKey = "dgfmcrypt";

        public static String EncryptString(String aStr)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(aStr);



            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(cryptKey));

            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice
            hashmd5.Clear();


            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;

            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);

            //Release resources held by TripleDes Encryptor
            tdes.Clear();

            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

        }

        public static string DecryptString(String cipherString)
        {
            byte[] keyArray;

            //get the byte code of the string
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);


            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(cryptKey));
            //release any resource held by the MD5CryptoServiceProvider

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;

            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);

            //Release resources held by TripleDes Encryptor                
            tdes.Clear();

            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static TAttribute GetAttribute<TAttribute>(Enum value) where TAttribute : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            return fi.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException();

            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description.ToUpper() == description.ToUpper())
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.ToUpper() == description.ToUpper())
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Value from description not found.", description);
        }

        public static DateTime previousWeekDay(this DateTime value)
        {
            DateTime dt = value;
            do
               dt = dt.AddDays(-1);
            while ((int)dt.DayOfWeek < 1 || (int)dt.DayOfWeek > 5) ;

            return dt;
        }

        public static DateTime UnixTimeStampToDateTime(Double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static String shortUrlId()
        {
            Guid guidId = Guid.NewGuid();
            var base64Guid = Convert.ToBase64String(guidId.ToByteArray());

            // Replace URL unfriendly characters
            base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

            // Remove the trailing ==
            return base64Guid.Substring(0, base64Guid.Length - 2);
        }

    }

}