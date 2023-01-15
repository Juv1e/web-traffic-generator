# Web Traffic Generator

This is a simple web traffic generator written in C#. It uses the RestSharp library to make HTTP requests and recursively browse a list of root URLs to a specified depth. The goal of this project is to simulate web traffic for testing purposes.

**This project was converted from Python to C# with the help of ChatGPT. It was made for learning purposes.**

## Configuration

All configuration options are located at the top of the `Config` class. They include:

- `MAX_DEPTH`: The maximum click depth to browse each root URL.
- `MIN_DEPTH`: The minimum click depth to browse each root URL.
- `MAX_WAIT`: The maximum amount of time to wait between HTTP requests.
- `MIN_WAIT`: The minimum amount of time to wait between HTTP requests.
- `DEBUG`: Set to true to enable useful console output.
- `ROOT_URLS`: A list of root URLs to browse.
- `blacklist`: A list of URLs or strings to exclude from browsing.
- `USER_AGENT`: A valid user agent string to use for HTTP requests.

## Usage

1. Clone or download the repository.
2. Open the solution in Visual Studio.
3. Edit the configuration options in the `Config.cs` file as desired.
4. Build and run the project.

## Acknowledgments

- Inspiration: [ReconInfoSec](https://github.com/ReconInfoSec/web-traffic-generator)
