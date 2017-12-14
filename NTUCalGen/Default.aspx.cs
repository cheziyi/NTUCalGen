using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace NTUCalGen
{
    public partial class Default : System.Web.UI.Page
    {
        //DateTime start;
        string icalOut = "";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnParse_Click(object sender, EventArgs e)
        {
            try
            {

                String title = WebConfigurationManager.AppSettings["fileTitle"];

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(txtSrc.Text);

                List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table")
                .Descendants("tr")
                .Skip(1)
                .Where(tr => tr.Elements("td").Count() > 1)
                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                .ToList();

                HtmlAgilityPack.HtmlNode bnode = doc.DocumentNode.SelectSingleNode("//b");
                title += bnode.InnerText;

                title += hidMatric.Value;

                for (int i = 0; i < table.Count; i++)
                {
                    if (CheckBlank(table, i, 0) && i != 0)
                    {
                        for (int j = 0; j < table[i].Count; j++)
                        {
                            if (CheckBlank(table, i, j))
                            {
                                table[i][j] = table[i - 1][j];
                            }
                        }
                    }
                }

                icalOut = "BEGIN:VCALENDAR" + "\r\n";
                icalOut += "PRODID: -//CCZY//ntucalgen.cczy.io//" + "\r\n";
                icalOut += "VERSION:2.0" + "\r\n";
                icalOut += "BEGIN:VTIMEZONE" + "\r\n";
                icalOut += "TZID:Singapore Standard Time" + "\r\n";
                icalOut += "BEGIN:STANDARD" + "\r\n";
                icalOut += "DTSTART:16010101T000000" + "\r\n";
                icalOut += "TZOFFSETFROM:+0800" + "\r\n";
                icalOut += "TZOFFSETTO:+0800" + "\r\n";
                icalOut += "END:STANDARD" + "\r\n";
                icalOut += "END:VTIMEZONE" + "\r\n";

                for (int i = 0; i < table.Count; i++)
                {
                    if (!CheckBlank(table, i, 11))
                    {
                        String dateStartStr = WebConfigurationManager.AppSettings["semStart"];

                        DateTime tempStart = DateTime.Parse(dateStartStr);
                        DateTime tempEnd = DateTime.Parse(dateStartStr);

                        int dayofweek = 0;
                        switch (table[i][11])
                        {
                            case "MON":
                                dayofweek = 0;
                                break;
                            case "TUE":
                                dayofweek = 1;
                                break;
                            case "WED":
                                dayofweek = 2;
                                break;
                            case "THU":
                                dayofweek = 3;
                                break;
                            case "FRI":
                                dayofweek = 4;
                                break;
                            default:
                                break;
                        }

                        tempStart = tempStart.AddDays(dayofweek);
                        tempEnd = tempEnd.AddDays(dayofweek);

                        if (chkAbbr.Checked)
                        {
                            table[i][1] = Abbr(table[i][1]);
                        }


                        if (table[i][14].StartsWith("Teaching Wk"))
                        {
                            table[i][14] = table[i][14].Substring(11);

                            string[] times = table[i][12].Split('-');

                            int start = Convert.ToInt32(times[0]);
                            tempStart = tempStart.AddHours(start / 100);
                            tempStart = tempStart.AddMinutes(start % 100);

                            int end = Convert.ToInt32(times[1]);
                            tempEnd = tempEnd.AddHours(end / 100);
                            tempEnd = tempEnd.AddMinutes(end % 100);




                            string[] days = table[i][14].Split(',');
                            foreach (string day in days)
                            {

                                if (day.Contains("-"))
                                {
                                    string[] days2 = day.Split('-');

                                    for (int k = Convert.ToInt32(days2[0]) - 1; k < Convert.ToInt32(days2[1]); k++)
                                    {
                                        int week = k;

                                        if (week > 6)
                                            week += 1;

                                        icalOut += GenerateICalEvent(tempStart.AddDays(week * 7), tempEnd.AddDays(week * 7), table[i]);
                                    }
                                }
                                else
                                {
                                    int week = Convert.ToInt32(day) - 1;
                                    if (week > 6)
                                        week += 1;

                                    icalOut += GenerateICalEvent(tempStart.AddDays(week * 7), tempEnd.AddDays(week * 7), table[i]);
                                }
                            }

                        }
                    }
                }

                icalOut += "END:VCALENDAR";

                Response.Clear();
                Response.ClearHeaders();

                Response.AddHeader("Content-Length", icalOut.Length.ToString());
                Response.ContentType = "text/plain";
                Response.AppendHeader("content-disposition", "attachment;filename=\"" + title + ".ics\"");

                Response.Write(icalOut);

                Response.End();

                //lblError.ForeColor = System.Drawing.Color.Green;
                //lblError.Text = "Your iCal is generated!";

                //Response.AddHeader("Content-disposition", "attachment; filename=" + title+ ".ics");
                //Response.ContentType = "text/plain";
                //Response.BinaryWrite(GetBytes(icalOut));
                //Response.End();

            }
            catch (Exception ex)
            {
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = "Error parsing html!";
            }
        }


        string GenerateICalEvent(DateTime start, DateTime end, List<string> lesson)
        {
            string iEvent = "";

            if (lesson[5].Equals("&nbsp;"))
                lesson[5] = "";

            iEvent += "BEGIN:VEVENT" + "\r\n";
            iEvent += "DTSTART;TZID=\"Singapore Standard Time\":" + ConvertToDateString(start) + "\r\n";
            iEvent += "DTEND;TZID=\"Singapore Standard Time\":" + ConvertToDateString(end) + "\r\n";
            iEvent += "LOCATION:" + lesson[13] + " (" + lesson[9] + ")" + "\r\n";
            iEvent += "SUMMARY:" + lesson[0] + "-" + lesson[1] + "\r\n";
            iEvent += "DESCRIPTION:" + "Map: http://maps.ntu.edu.sg/m?q=" + lesson[13] + "\\nGroup: " + lesson[10] + "\\n" + "Index: " + lesson[6] + "\\n" + "Course Type: " + lesson[3] + "\\n" + "AU: " + lesson[2] + "\\n" + lesson[5] + "\\n" + "\r\n";
            iEvent += "END:VEVENT" + "\r\n";

            return iEvent;
        }

        byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        string ConvertToDateString(DateTime dt)
        {
            return dt.ToString("yyyyMMddTHHmmss");
        }

        Boolean CheckBlank(List<List<string>> data, int i, int j)
        {
            if (data[i][j].Equals("&nbsp;") || data[i][j].Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        string Abbr(string subject)
        {
            string[] words = subject.Split(' ');
            subject = "";
            foreach (string word in words)
            {
                if (word.Length > 4)
                {
                    subject += word.Substring(0, 4) + " ";
                }
                else
                {
                    subject += word + " ";
                }

            }
            return subject;
        }
    }
}