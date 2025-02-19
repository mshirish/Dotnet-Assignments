﻿namespace HttpClientExample;

class Program
{
	static async Task Main(string[] args) 
	{
		//Creating an instance of HttpClient
		using (HttpClient httpClient = new HttpClient())
		{ 
			string url = "https://api-football-v1.p.rapidapi.com/v2/odds/league/865927/bookmaker/5?page=2"; // the api url
			Console.WriteLine("Request URL: " + url);
			try 
			{
				//sending a GET request to the url
				HttpResponseMessage request = await httpClient.GetAsync(url);

				request.Headers.Add("x-rapidapi-key", "your-api-key-here"); // adding headers

				//Check if response is successfull
				request.EnsureSuccessStatusCode();

				//reading the response data as string
				string responseData = await request.Content.ReadAsStringAsync();

				Console.WriteLine("Response Data:");
				Console.WriteLine(responseData);
			}
			catch (HttpRequestException e)
			{
				// Handle any request errors
				Console.WriteLine($"Request error: {e.Message}");
			}
		}

		Console.ReadKey();
	}
}
