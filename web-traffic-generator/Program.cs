using System.Net;
using System.Text.RegularExpressions;
using RestSharp;

namespace web_traffic_generator
{
    class Program
    {
        public static long data_meter;
        public static int good_requests;
        public static int bad_requests;
        

        static async Task Main(string[] args)
        {
            // Check if the file 'root_urls.txt' exists
            if (!File.Exists("root_urls.txt")) {
                // Create the file and write the default URLs to it
                File.WriteAllLines("root_urls.txt", new string[] { 
                    "https://digg.com/",
                    "https://www.yahoo.com",
                    "https://www.reddit.com",
                    "http://www.cnn.com",
                    "http://www.ebay.com",
                    "https://en.wikipedia.org/wiki/Main_Page",
                    "https://austin.craigslist.org/"
                    
                });
                Console.WriteLine("Please fill the 'root_urls.txt' file with your own website links, each website should be on a new line.");
                Console.ReadKey();
                return;
            }
            // Read the URLs from the text file and assign them to ROOT_URLS
            Config.ROOT_URLS = File.ReadAllLines("root_urls.txt");
            data_meter = 0;
            good_requests = 0;
            bad_requests = 0;

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("Traffic generator started");
            Console.WriteLine("Diving between " + Config.MIN_DEPTH + " and " + Config.MAX_DEPTH + " links deep into " + Config.ROOT_URLS.Length + " root URLs,");
            Console.WriteLine("Waiting between " + Config.MIN_WAIT + " and " + Config.MAX_WAIT + " seconds between requests. ");
            Console.WriteLine("This script will run indefinitely. Ctrl+C to stop.");

            while (true)
            {
                DebugPrint("Randomly selecting one of " + Config.ROOT_URLS.Length + " Root URLs");
                string random_url = Config.ROOT_URLS[new Random().Next(0, Config.ROOT_URLS.Length)];
                int depth = new Random().Next(Config.MIN_DEPTH, Config.MAX_DEPTH);

                RecursiveBrowse(random_url, depth);
            }
        }
        private static void RecursiveBrowse(string url, int depth)
        {
            DebugPrint("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            DebugPrint("Recursively browsing [" + url + "] ~~~ [depth = " + depth + "]");

            if (depth == 0)
            {
                DoRequest(url);
                return;
            }
            else
            {
                var page = DoRequest(url);

                if (page == null)
                {
                    DebugPrint("  Stopping and blacklisting: page error");
                    Config.BLACKLIST.Add(url);
                    return;
                }

                DebugPrint("  Scraping page for links");
                var valid_links = GetLinks(page);
                DebugPrint("  Found " + valid_links.Length + " valid links");

                if (!valid_links.Any())
                {
                    DebugPrint("  Stopping and blacklisting: no links");
                    Config.BLACKLIST.Add(url);
                    return;
                }

                var sleep_time = new Random().Next(Config.MIN_WAIT, Config.MAX_WAIT);
                DebugPrint("  Pausing for " + sleep_time + " seconds...");
                Thread.Sleep(sleep_time * 1000);

                RecursiveBrowse(valid_links[new Random().Next(0, valid_links.Length)], depth - 1);
            }
        }
        public static void DebugPrint(string message)
        {
            if (Config.DEBUG)
            {
                Console.WriteLine($"> {message}");
            }
        }

        public static string[] GetLinks(IRestResponse page)
        {
            var pattern = "(?:href=\")(https?://[^\"]+)(?:\")";
            var links = Regex.Matches(page.Content, pattern)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();

            links = links.Where(link => !Config.BLACKLIST.Any(blacklist_item => link.Contains(blacklist_item))).ToArray();

            return links;
        }

        public static string HrBytes(long bytes_, string suffix = "B", bool si = false)
        {
            long bits = si ? 1024 : 1000;

            for (string unit = ""; unit != "Y"; unit = unit.Length == 0 ? "" : unit + "i")
            {
                if (Math.Abs(bytes_) < bits)
                {
                    return string.Format("{0:F1}{1}{2}", bytes_, unit, suffix);
                }

                bytes_ /= bits;
            }

            return string.Format("{0:F1}{1}{2}", bytes_, "Y", suffix);
        }
        public static IRestResponse DoRequest(string url)
        {
            DebugPrint("  Requesting page...");

            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("User-Agent", Config.USER_AGENT);

            try
            {
                var response = client.Execute(request);

                var page_size = response.Content.Length;
                data_meter += page_size;

                DebugPrint("  Page size: " + HrBytes(page_size));
                DebugPrint("  Data meter: " + HrBytes(data_meter));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    bad_requests++;
                    DebugPrint("  Response status: " + response.StatusCode);
                    if (response.StatusCode == (HttpStatusCode) 429)
                    {
                        DebugPrint("  We're making requests too frequently... sleeping longer...");
                        Config.MIN_WAIT += 10;
                        Config.MAX_WAIT += 10;
                    }
                }
                else
                {
                    good_requests++;
                }
                DebugPrint("  Good requests: " + good_requests);
                DebugPrint("  Bad reqeusts: " + bad_requests);

                return response;
            }
            catch (Exception)
            {
                Thread.Sleep(30 * 1000);
                return null;
            }
            }
        }
    }