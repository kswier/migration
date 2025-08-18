using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;

namespace Library
{
    public enum UserDeviceEnum
    {
        computer, winpc, mac, wintablet, ipad, tablet, mobile
    }

    public class UserCountryAttribute : Attribute
    {
        internal String moneySymbol;
        internal String culture;
        internal String dateFormat;
        internal String longDateTimeFormat;
        internal String uiDateFormat;
        internal String chartDateFormat;


        public UserCountryAttribute(String moneySymbol, String culture, String dateFormat, String longDateTimeFormat, String uiDateFormat, String chartDateFormat)
        {
            this.moneySymbol = moneySymbol;
            this.culture = culture;
            this.dateFormat = dateFormat;
            this.longDateTimeFormat = longDateTimeFormat;
            this.uiDateFormat = uiDateFormat;
            this.chartDateFormat = chartDateFormat;
        }
    }

    public enum UserCountryEnum
    {
        [UserCountryAttribute("$", "en-US", "MM/dd/yyyy", "MMM dd, yyyy hh:mm tt", "mm/dd/yyyy", "MM/dd/yy")]
        US,
        [UserCountryAttribute("", "en-GB", "dd/MM/yyyy", "MMM dd, yyyy hh:mm tt", "dd/mm/yyyy", "dd/MM/yy")]
        UK,
        [UserCountryAttribute("$", "en-CA", "dd/MM/yyyy", "MMM dd, yyyy hh:mm tt", "dd/mm/yyyy", "dd/MM/yy")]
        CA

    }

    public class User
    {
        public String userId;
        public String email;
        public String password;
        public String passwordEncrypted;
        public String lastName;
        public String firstName;
        public String ssn;
        public UserCountryEnum country = UserCountryEnum.US;
        public bool isLoggedIn = false;
        private bool loaded = false;
        public UserDeviceEnum userDevice;
        private DateTime createDate;

        public User()
        {
        }

        public String userAgent
        {
            set
            {
                String userAgentStr = value.ToUpper();
                if ((userAgentStr.IndexOf("WINDOWS") >= 0) && (userAgentStr.IndexOf("TABLET") >= 0))
                    userDevice = UserDeviceEnum.wintablet;
                else
                    if (userAgentStr.IndexOf("IPAD") >= 0)
                    userDevice = UserDeviceEnum.ipad;
                else
                        if (userAgentStr.IndexOf("TABLET") >= 0)
                    userDevice = UserDeviceEnum.tablet;
                else
                            if (userAgentStr.IndexOf("MAC") >= 0)
                    userDevice = UserDeviceEnum.mac;
                else
                                if (userAgentStr.IndexOf("WINDOWS") >= 0)
                    userDevice = UserDeviceEnum.winpc;
                else
                    userDevice = UserDeviceEnum.computer;
            }
        }


        public bool loadByEmail(ADODB db, String emailAddress, String userAgent, bool isMobile = false)
        {
            int rslt = 0;
            ADOQuery qry = new ADOQuery(db);
            String sql = "select * from userTbl where email = " + Utils.quotedString(emailAddress);
            qry.open(sql);
            while (!qry.eof)
            {
                rslt++;
                loadFromQry(qry);
                this.userAgent = userAgent;
                if (isMobile)
                    userDevice = UserDeviceEnum.mobile;
                loaded = true;
                qry.next();
            }
            qry.close();

            return (rslt > 0);
        }

        public bool isValidPassword(String pwd)
        {
            bool rslt = false;
            if (pwd.ToUpper() == password.ToUpper())
                rslt = true;

            return rslt;
        }

        public bool loadByUserId(ADODB db, String uid, String token, String userAgent)
        {
            int rslt = 0;
            ADOQuery qry = new ADOQuery(db);
            String sql = "select * from userTbl where userId = " + Utils.quotedString(uid);
            sql += " and password = " + token.quotedString();
            qry.open(sql);
            while (!qry.eof)
            {
                rslt++;
                loadFromQry(qry);
                this.userAgent = userAgent;
                loaded = true;
                qry.next();
            }
            qry.close();

            return (rslt > 0);
        }

        public bool loadByUserId(ADODB db, String uid, String userAgent = "")
        {
            int rslt = 0;
            ADOQuery qry = new ADOQuery(db);
            String sql = "select * from userTbl where userId = " + Utils.quotedString(uid);
            qry.open(sql);
            while (!qry.eof)
            {
                rslt++;
                loadFromQry(qry);
                this.userAgent = userAgent;
                loaded = true;
                qry.next();
            }
            qry.close();

            return (rslt > 0);
        }

        internal void loadFromQry(ADOQuery qry)
        {
            userId = qry.fieldAsString("userId");
            email = qry.fieldAsString("email");
            passwordEncrypted = qry.fieldAsString("password");
            password = Utils.DecryptString(passwordEncrypted);
            lastName = qry.fieldAsString("lastName");
            firstName = qry.fieldAsString("firstName");
            ssn = qry.fieldAsString("ssn");
            country = (UserCountryEnum)Enum.Parse(typeof(UserCountryEnum), qry.fieldAsString("country"));
        }

        public bool emailExists(ADODB db, String emailAddress)
        {
            ADOQuery qry = new ADOQuery(db);
            String sql = "select * from userTbl where email = " + Utils.quotedString(emailAddress);
            bool exists = false;
            qry.open(sql);
            while (!qry.eof)
            {
                exists = true;
                qry.next();
            }

            qry.close();
            return exists;
        }

        public void save(ADODB db, HttpRequest request)
        {
            email = request.Form["email"];
            if (request.Form["isencrypted"] == "1")
            {
                passwordEncrypted = request.Form["password"];
            }
            else
            {
                password = request.Form["password"];
                passwordEncrypted = Utils.EncryptString(password);
            }
            lastName = request.Form["lastName"];
            firstName = request.Form["firstName"];
            ssn = request.Form["ssn"];
            bool isMobile = request.Form["ismobile"] == "1";


            if (userId != null)
            {
                String sql = "update userTbl set";
                sql += " email = " + email.quotedString();
                sql += ",lastName = " + lastName.quotedNullString();
                sql += ",firstName = " + firstName.quotedNullString();
                sql += ",ssn = " + ssn.quotedNullString();
                sql += ",password = " + passwordEncrypted.quotedString();
                sql += " where userId = " + userId.ToString().quotedString();
                ADOQuery qry = new ADOQuery(db);
                qry.execSQL(sql);
                qry.close();
            }
            else
            {
                userId = System.Guid.NewGuid().ToString();
              
                createDate = DateTime.Now;
                String sql = "insert into userTbl (userId, email, password, lastName, firstName, ssn, createDt, country, createIp, mobileReg) values (";
                sql += userId.ToString().quotedString() + "," + email.quotedString() + "," + passwordEncrypted.quotedString() + ",";
                sql += lastName.quotedNullString() + "," + firstName.quotedNullString() + "," + ssn.quotedNullString() + "," + createDate.quotedNullDateTime(Utils.quoteType.qtSingle);
                sql += "," + country.ToString().quotedString() + ',' + request.UserHostAddress.quotedNullString() + "," + Convert.ToInt32(isMobile).ToString();
                sql += ")";
                ADOQuery qry = new ADOQuery(db);
                qry.execSQL(sql);
                qry.close();
            }
        }

        public String fullName
        {
            get
            {
                if (String.IsNullOrEmpty(firstName) && String.IsNullOrEmpty(lastName))
                    return email;
                else
                    return firstName + " " + lastName;
            }
        }

        public void createNewUser(ADODB db, HttpRequest request)
        {
            String formEmail = request.Form["email"];
            if (!emailExists(db, formEmail))
            {
                save(db, request);
            }
        }

        
        public void loggedIn(ADODB db)
        {
            String sql = "";
            try
            {
                isLoggedIn = true;
                DateTime loginDate = DateTime.Now;
                sql = "update userTbl set";
                sql += " loginDt = " + loginDate.quotedNullDateTime(Utils.quoteType.qtSingle);
                sql += " where userId = " + userId.ToString().quotedString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Building sql " + ex.Message);
            }

            try
            {
                ADOQuery qry = new ADOQuery(db);
                qry.execSQL(sql);
                qry.close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Exec sql " + ex.Message);
            }

        }
    }
}