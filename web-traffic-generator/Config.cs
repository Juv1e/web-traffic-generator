using System.Collections.Generic;

namespace web_traffic_generator;

public class Config
{

    public static int MAX_DEPTH = 15; // maximum click depth
    public static int MIN_DEPTH = 3; // minimum click depth
    public static int MAX_WAIT = 25; // maximum amount of time to wait between HTTP requests
    public static int MIN_WAIT = 10; // minimum amount of time allowed between HTTP requests
    public static bool DEBUG = true; // set to true to enable useful console output

    public static string[] ROOT_URLS;

    public static List<string> BLACKLIST = new List<string> {
        "https://t.co", 
        "t.umblr.com", 
        "messenger.com", 
        "itunes.apple.com", 
        "l.facebook.com", 
        "bit.ly", 
        "mediawiki", 
        ".css", 
        ".ico", 
        ".xml", 
        "intent/tweet", 
        "twitter.com/share", 
        "signup", 
        "login", 
        "dialog/feed?", 
        ".png", 
        ".jpg", 
        ".json", 
        ".svg", 
        ".gif", 
        "zendesk",
        "clickserve"
    };

    public static string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

}