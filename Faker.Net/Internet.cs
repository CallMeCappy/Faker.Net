using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using Faker.Random;

namespace Faker
{
    public class Internet : FakerBase
    {
        private static char[] punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
        private static char[] startingChars = new char[] { '<', '&' };

        public Internet()
        { }
        public Internet(LocaleType type) : base(type) { }

        // default static interface
        public static Internet Default { get { return defaultValue; } }
        private static Internet defaultValue = new Internet();

        public virtual string GetPassword() { return this.GetPassword(15); }
        public virtual string GetPassword(int length)
        {
            return this.GeneratePassword(length, length >> 2);
        }

        /// <summary>
        /// Source taken from https://github.com/Microsoft/referencesource/tree/master/System.Web/Security
        /// </summary>
        private string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            if (length < 1 || length > 128)
            {
                throw new ArgumentException("Value should be between 1 and 128", nameof(length));
            }

            if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
            {
                throw new ArgumentException("Value should be between 0 and length",
                    nameof(numberOfNonAlphanumericCharacters));
            }

            string password;
            int index;
            byte[] buf;
            char[] cBuf;
            int count;

            do
            {
                buf = new byte[length];
                cBuf = new char[length];
                count = 0;

                (new RNGCryptoServiceProvider()).GetBytes(buf);

                for (int iter = 0; iter < length; iter++)
                {
                    int i = (int)(buf[iter] % 87);
                    if (i < 10)
                        cBuf[iter] = (char)('0' + i);
                    else if (i < 36)
                        cBuf[iter] = (char)('A' + i - 10);
                    else if (i < 62)
                        cBuf[iter] = (char)('a' + i - 36);
                    else
                    {
                        cBuf[iter] = punctuations[i - 62];
                        count++;
                    }
                }

                if (count < numberOfNonAlphanumericCharacters)
                {
                    int j, k;
                    var rand = new System.Random();

                    for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
                    {
                        do
                        {
                            k = rand.Next(0, length);
                        }
                        while (!Char.IsLetterOrDigit(cBuf[k]));

                        cBuf[k] = punctuations[rand.Next(0, punctuations.Length)];
                    }
                }

                password = new string(cBuf);
            }
            while (IsDangerousString(password, out index));

            return password;
        }

        private static bool IsDangerousString(string s, out int matchIndex)
        {
            //bool inComment = false;
            matchIndex = 0;

            for (int i = 0; ;)
            {

                // Look for the start of one of our patterns
                int n = s.IndexOfAny(startingChars, i);

                // If not found, the string is safe
                if (n < 0) return false;

                // If it's the last char, it's safe
                if (n == s.Length - 1) return false;

                matchIndex = n;

                switch (s[n])
                {
                    case '<':
                        // If the < is followed by a letter or '!', it's unsafe (looks like a tag or HTML comment)
                        if (IsAtoZ(s[n + 1]) || s[n + 1] == '!' || s[n + 1] == '/' || s[n + 1] == '?') return true;
                        break;
                    case '&':
                        // If the & is followed by a #, it's unsafe (e.g. &#83;)
                        if (s[n + 1] == '#') return true;
                        break;
                }

                // Continue searching
                i = n + 1;
            }
        }

        private static bool IsAtoZ(char c) {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public virtual string GetAvatarURL()
        {
            return @"https://s3.amazonaws.com/uifaces/faces/twitter/" + Selector.GetRandomItemFromList(this.locale.AvatarURL);
        }

        public virtual Color GetColorRGB()
        {
            return Color.FromArgb(RandomProxy.Next(256), RandomProxy.Next(256), RandomProxy.Next(256));
        }

        public virtual Color GetColorARGB()
        {
            return Color.FromArgb(RandomProxy.Next(256), RandomProxy.Next(256), RandomProxy.Next(256), RandomProxy.Next(256));
        }

        public virtual string GetColorString()
        {
            return string.Format("#{0:X8}", this.GetColorARGB().ToArgb());
        }

        public virtual string GetUserName()
        {
            return this.GetUserName(Name.Default.GetFirstName(), Name.Default.GetLastName());
        }

        public virtual string GetUserName(string firstName, string lastName)
        {
            switch (RandomProxy.Next(3))
            {
                case 0:
                    return firstName + RandomProxy.Next(100);
                case 1:
                    return string.Format("{0}{1}{2}", firstName, Selector.GetRandomItemFromList(new[] { ".", "_" }),
                        lastName);
                default:
                    return string.Format("{0}{1}{2}{3}", firstName, Selector.GetRandomItemFromList(new[] { ".", "_" }),
                        lastName, RandomProxy.Next(100));
            }
        }

        public virtual string GetEmail(string firstName, string lastName)
        {
            return this.GetUserName(firstName, lastName) + "@" + Selector.GetRandomItemFromList(locale.FreeEmailDomain);
        }

        public virtual string GetEmail()
        {
            return this.GetEmail(Name.Default.GetFirstName(), Name.Default.GetLastName());
        }

        public virtual string GetIP()
        {
            return string.Format("{0}.{1}.{2}.{3}", RandomProxy.Next(256), RandomProxy.Next(256), RandomProxy.Next(256),
                RandomProxy.Next(256));
        }

        public virtual string GetDomainWord()
        {
            return Selector.GetRandomItemFromList(locale.FirstName).ToLower();
        }

        public virtual string GetDomainName()
        {
            return this.GetDomainWord() + "." + this.GetDomainSuffix();
        }

        public virtual string GetDomainSuffix()
        {
            return Selector.GetRandomItemFromList(locale.DomainSuffix);
        }

        private string randInt(int minValue, int maxValue)
        {
            return RandomProxy.Next(minValue, maxValue).ToString();
        }

        private string GetUserAgent()
        {
            // Code adopted from http://www.hackforums.net/archive/index.php/thread-1568584.html
            // Also update some new versions

            string[] arrBrowsers = {"Firefox/0."+randInt(7,9)+"."+randInt(0,5),
                                    "Outlook-Express/" + randInt(5,8) + "." + randInt(0,5),
                                    "Opera/"+randInt(7,9) + "." + randInt(1,80),
                                    "Safari/4"+randInt(1,15) + "." + randInt(1,6),
                                    "Arora/0."+randInt(1,5),
                                    "Gecko/200"+randInt(0,9024)+"Firefox/3."+randInt(1,6),
                                    "Lynx/2."+randInt(1,8)+"."+randInt(2,5)+"dev."+randInt(1,16),
                                    "Chrome/" + randInt(10, 35) + ".0."+randInt(0,154)+"."+randInt(0,60)};

            string[] arrOS = {  "Windows NT 5.1",
                                "Windows NT 6.1",
                                "Windows NT 6.0",
                                "Windows NT 6.3",
                                "Windows",
                                "Windows 95",
                                "Windows 98",
                                "FreeBSD i686",
                                "Ubuntu i686",
                                "Macintosh",
                                "Linux"};

            string[] arrPlugins = {   "MS-RTC LM "+randInt(4,8)+";",
                                      "Win64;",
                                      "x64;",
                                      "Trident/"+randInt(4,6)+".0;",
                                      "SLCC2;",
                                      ".NET CLR "+randInt(2,4)+"."+randInt(1,7)+".30"+randInt(50,500)+";",
                                      "Media Center PC "+randInt(0,6)+".0;",
                                      "Tablet PC "+randInt(1,3)+".0;",
                                      "Presto/2."+randInt(0,2)+"."+randInt(0,9),
                                      "rv:1."+randInt(1,5)+"."+randInt(1,9)+";",
                                      "WOW64;",
                                      "GTB"+randInt(5,7)+";",
                                      "WebTV/2."+randInt(0,9)+";",
                                      "AppleWebKit/"+randInt(500,550)+"."+randInt(1,50)+" (KHTML, like Gecko)",
                                      "Zune "+randInt(1,4)+".0;",
                                      "msn OptimizedIE"+randInt(5,8)+";",
                                      "OfficeLiveConnector.1."+randInt(1,9)+";",
                                      "OfficeLivePatch.0."+randInt(0,5)+";",
                                      "AskTB5."+randInt(0,7),
                                      "MS-RTC LM "+randInt(5,8)+";",
                                      "InfoPath."+randInt(1,4)+";",
                                      ".NET"+randInt(2,4)+".0C;"};

            string strPlugins = String.Empty;
            int plugincount = RandomProxy.Next(3, 6);
            int[] history = new int[plugincount];
            for (int i = 0; i != plugincount; i++)
            {
                int ran = RandomProxy.Next(0, arrPlugins.Length);
                if (!history.Contains(ran))
                {
                    history[i] = ran;
                    strPlugins = strPlugins + " " + arrPlugins[ran];
                }
                else
                {
                    i--;
                }
            }
            string Type = "Mozilla/" + randInt(4, 6) + ".0";
            string OS = arrOS[RandomProxy.Next(0, arrOS.Length)];
            string[] arrCompatible = { "compatible; MSIE " + randInt(6, 10) + ".0; ", "", "" };
            string Compatible = arrCompatible[RandomProxy.Next(0, 1)];
            if (OS.IndexOf("Linux") != -1 || OS.IndexOf("Ubuntu") != -1 || OS.IndexOf("FreeBSD") != -1 || OS.IndexOf("Macintosh") != -1) { Compatible = ""; } //remove the compatible MSIE message from the string
            return Type + " (" + Compatible + OS + ";" + strPlugins + " " + arrBrowsers[RandomProxy.Next(0, arrBrowsers.Length)] + " )";
        }


    }
}
