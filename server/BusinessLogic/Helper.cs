using Server.DataAccess;
using Server.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO.Compression;

namespace Server.BusinessLogic
{
    public class Helper
    {
        private static Random random = new Random();

        public static long NowUtc
        {
            get
            {
                return DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

        public static string GetMonthName(int value)
        {
            switch (value)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";

                default:
                    return "Jan";
            }
        }

        public static int GetDateKey(DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }

        public static long ToUTC(DateTime? dateTime)
        {
            try
            {
                if (dateTime == null) return 0;
                var _dateTime = (DateTime)dateTime;
                _dateTime = DateTime.SpecifyKind(_dateTime, DateTimeKind.Local);
                DateTimeOffset offset = _dateTime;
                return offset.ToUnixTimeMilliseconds();
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        public static string Web(string url)
        {
            string pageContent = string.Empty;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:8.0) Gecko/20100101 Firefox/8.0";
                webRequest.Referer = "http://www.google.com.vn/";

                WebResponse response = webRequest.GetResponse();
                System.IO.Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                pageContent = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                pageContent = "Error:" + e.Message + "\n" + e.StackTrace;
            }

            return pageContent;
        }

        public static string GetMd5Hash(string input)
        {
            try
            {
                MD5 md5Hash = MD5.Create();
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        public static List<T> FromCSV<T>(string csv)
        {
            try
            {
                if (string.IsNullOrEmpty(csv))
                {
                    return new List<T>();
                }

                List<T> result = new List<T>();
                string[] tokens = csv.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string token in tokens)
                {
                    T t = (T)Convert.ChangeType(token, typeof(T));
                    result.Add(t);
                }

                return result;
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        public static string ToCSV<T>(List<T> values)
        {
            try
            {
                if (values == null || values.Count == 0)
                {
                    return string.Empty;
                }

                string result = string.Empty;
                foreach (T value in values)
                {
                    result += "," + value.ToString();
                }

                return result.Substring(1);
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        public static string Serialize(object dataToSerialize)
        {
            try
            {
                if (dataToSerialize == null) return null;

                using (StringWriter stringwriter = new StringWriter())
                {
                    var serializer = new XmlSerializer(dataToSerialize.GetType());
                    serializer.Serialize(stringwriter, dataToSerialize);
                    return stringwriter.ToString();
                }
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        public static T Deserialize<T>(string xmlText)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(xmlText)) return default(T);

                using (StringReader stringReader = new StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception e)
            {
                //LogBC.Instance.Log(e);
                throw;
            }
        }

        /// <summary>
        /// Implementation of Levenshtein algorithm
        /// https://en.wikipedia.org/wiki/Levenshtein_distance
        /// </summary>
        /// <param name="s">string 1</param>
        /// <param name="t">string 2</param>
        /// <returns>The result distance</returns>
        public static int LevenshteinDistance(string s, string t)
        {
            if (s == null)
            {
                if (t == null) return 0; else return t.Length;
            }

            if (t == null)
            {
                if (s == null) return 0; else return s.Length;
            }

            int m = s.Length;
            int n = t.Length;
            int[,] d = new int[m + 1, n + 1];

            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= n; j++)
            {
                for (int i = 1; i <= m; i++)
                {
                    if (s[i - 1] == t[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = Minimum(
                            d[i - 1, j] + 1,  // a deletion
                            d[i, j - 1] + 1,  // an insertion
                            d[i - 1, j - 1] + 1 // a substitution
                        );
                    }
                }
            }

            return d[m, n];
        }

        public static int Minimum(int a, int b, int c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        public static string Contains(string column, int entityId)
        {
            return $"[{column}] = '{entityId}' " +
                     $"OR [{column}] LIKE '{entityId + ","} % ' " +
                     $"OR [{column}] LIKE '{"%," + entityId + ","}% ' " +
                     $"OR [{column}] LIKE '{"%," + entityId} ' ";
        }

        public static int GetPageCount(int totalCount, int itemPerPage)
        {
            if (itemPerPage == 0) return 0;
            int n = totalCount / itemPerPage;
            int d = totalCount % itemPerPage;
            return (d == 0) ? n : n + 1;
        }

        private static async Task<string> GetResponse(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                //request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                request.Headers.TryAddWithoutValidation("User-Agent", "Googlebot/2.1 (+http://www.googlebot.com/bot.html)");
                request.Headers.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                HttpClient _HttpClient = new HttpClient();

                using (var response = await _HttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return result;

                    //using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    //using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                    //using (var streamReader = new StreamReader(decompressedStream))
                    //{
                    //    return await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    //}
                }
            }
        }

        public async static Task<string> GetImageAsBase64Url(string url)
        {
            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(url);
                string mime = "image/jpeg";
                if (url.ToLower().EndsWith(".png")) mime = "image/png";
                return $"data:{mime};base64," + Convert.ToBase64String(bytes);
            }
        }

        private static string[] Gen4(string[] words)
        {
            int i = new Random().Next(words.Length);
            return new string[] {
                words[i],
                words[(i + 1) % words.Length],
                words[i + 2],
                words[i + 3],
            };
        }

        public static void SeedData()
        {
            SeedUsers();
            SeedQuestions();
            SeedQuizes();
        }

        public static void SeedUsers()
        {
            var qs = UserDAC.Instance.CountByFields(new string[] { }, new object[] { });
            if (qs > 0) return;

            for (int i = 0; i < 10; i++)
            {
                User user = new User()
                {
                    Email = $"user{i}@abc.com",
                    PasswordHash = "0cc175b9c0f1b6a831c399e269772661",
                    Name = $"User {i}",
                };
                UserDAC.Instance.Insert(user);
            }
        }

        public static void SeedQuizes()
        {
            var qs = QuizDAC.Instance.CountByFields(new string[] { }, new object[] { });
            if (qs > 0) return;

            List<Question> questions = QuestionDAC.Instance.GetAll();

            for (int i = 0; i < 5; i++)
            {
                List<string> questionCodes = new List<string>();

                for (int j = 0; j < 7; j++)
                {
                    int qi = new Random().Next(questions.Count);
                    if (!questionCodes.Contains(questions[qi].Code))
                    {
                        questionCodes.Add(questions[qi].Code);
                    }
                }

                Quiz quiz = new Quiz()
                {
                    Code = $"QUIZ_{i}",
                    Questions = questionCodes,
                };
                QuizDAC.Instance.Insert(quiz);
            }
        }

        public static void SeedQuestions()
        {
            var qs = QuestionDAC.Instance.CountByFields(new string[] { }, new object[] { });
            if (qs > 0) return;

            string[] wordENs = new string[] { "apple", "banana", "candy", "door", "emergency", "fun", "goat", "house", "ice" };
            string[] wordVNs = new string[] { "Táo", "Chuối", "Kẹo", "Cửa", "Khẩn cấp", "Vui vẻ", "Con dê", "Nhà", "Nước đá" };
            var answers = Gen4(wordVNs);

            for (int i = 0; i < wordENs.Length; i++)
            {
                Question question = new Question()
                {
                    Code = $"QUE_{i}",
                    Description = $"What does '{wordENs[i]}' mean?",
                    Answer = $"{wordVNs[i]}",
                    AnswerA = answers[0],
                    AnswerB = answers[1],
                    AnswerC = answers[2],
                    AnswerD = answers[3],
                };
                int m = i % 4;
                if (m == 0) question.AnswerA = question.Answer;
                if (m == 1) question.AnswerB = question.Answer;
                if (m == 2) question.AnswerC = question.Answer;
                if (m == 3) question.AnswerD = question.Answer;
                QuestionDAC.Instance.Insert(question);
            }
        }

    }
}

