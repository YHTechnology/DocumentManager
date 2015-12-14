using System;
using System.IO;
using System.Web;
using Microsoft.Win32;
using System.Text;


namespace DocumentManager.Web
{
    public class LocalServerService
    {

        /// <summary>
        /// if no local file and reg info, create local file and reg info
        /// if has reg info, but no local file, consider as local file has been deleted intentionally, return -1100
        /// if has local file, but no reg info, consider as reg info has been deleted intentionally, return -1000
        /// if can create reg info, but can't create local file, return -1200
        /// if can't create reg info, return -1300
        /// if has reg info and local file, calculate the interval days of recorded datetime and now.
        /// </summary>
        /// <returns></returns>
        public int GetExpireDay()
        {
            //string str1 = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            //string filepath = Path.Combine(str1, "serverService");

            //if (!File.Exists(filepath))
            //{
            //    //return -1100;


            //    if (!HasRegInfo())
            //    {
            //        //no local file and reg info
            //        //create local file and reg info
            //        if (CreateRegInfo())
            //        {
            //            using (StreamWriter sw = new StreamWriter(filepath))
            //            {
            //                try
            //                {
            //                    sw.WriteLine(encodestring(DateTime.Now.AddDays(365).ToString()));
            //                    return 364;
            //                }
            //                catch
            //                {
            //                    //can create reg info, but can't create local file, return -1200
            //                    RegistryKey rk = Registry.CurrentUser;
            //                    RegistryKey software = rk.OpenSubKey("SOFTWARE", true);
            //                    software.DeleteSubKey("DMServerService");
            //                    return -1200;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //can't create reg info, return -1300
            //            return -1300;
            //        }
            //    }

            //    else
            //    {
            //        //has reg info, but no local file
            //        //consider as local file has been deleted intentionally, return -1100
            //        return -1100;
            //    }
            //}
            //else
            //{
            //    //using (StreamReader sr = new StreamReader(filepath))
            //    //{
            //    //    string tempstr1 = sr.ReadLine();
            //    //    DateTime recordTime = DateTime.Parse(decodestring(tempstr1));
            //    //    TimeSpan ts = recordTime - DateTime.Now;
            //    //    return ts.Days;
            //    //}

            //    if (HasRegInfo())
            //    {
            //        using (StreamReader sr = new StreamReader(filepath))
            //        {
            //            string tempstr1 = sr.ReadLine();
            //            DateTime recordTime = DateTime.Parse(decodestring(tempstr1));

            //            TimeSpan ts = recordTime - DateTime.Now;

            //            return ts.Days;

            //        }
            //    }
            //    else
            //    {
            //        //has local file, but no reg info
            //        //consider as reg info has been deleted intentionally, return -1000
            //        return -1000;
            //    }
            //}

            DateTime expirtDate = new DateTime(2017, 1, 1);
            TimeSpan remainDate = expirtDate - DateTime.Now;
            return remainDate.Days;

        }

        /*
        private bool HasRegInfo()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser;

                //RegistryKey lm = Registry.LocalMachine;
                RegistryKey software = rk.OpenSubKey("SOFTWARE", true);
                RegistryKey dms;
                dms = software.OpenSubKey("DMServerService", true);
                if (dms == null)
                {
                    return false;
                }
            }
            catch(Exception exe)
            {
                string str1 = exe.Message;
                return false;
            }

            return true;
        }

        private bool CreateRegInfo()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser;
                //RegistryKey lm = Registry.LocalMachine;
                RegistryKey software = rk.OpenSubKey("SOFTWARE", true);

                RegistryKey product = software.CreateSubKey("DMServerService");
            }
            catch
            {
                return false;
            }

            return true;

        }

        */
        private string encodestring(string aStr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = aStr.Length - 1; i >= 0; i--)
            {
                sb.Append((char)(aStr[i] + i));
            }

            return sb.ToString();
        }

        private string decodestring(string aStr)
        {
            StringBuilder sb = new StringBuilder();

            int j = 0;
            for (int i = aStr.Length - 1; i >= 0; i--)
            {
                sb.Append((char)(aStr[i] - j));
                j++;
            }

            return sb.ToString();
        }
    }
}