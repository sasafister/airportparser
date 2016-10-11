using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.Write("From (ex. ZAG, BOS, LHR): ");
			string from = Console.ReadLine();
			Console.Write("To (ex. ZAG, BOS, LHR): ");
			string to = Console.ReadLine();
			Console.Write("Departure date: (ex. 2016-10-09): ");
			string departure = Console.ReadLine();
			Console.Write("Return date: (ex. 2016-12-15): ");
			string returnDate = Console.ReadLine();
			Console.Write("Currency: (ex. HRK): ");
			string currency = Console.ReadLine();
			Console.Write("Adults: (ex. 1): ");
			string adults = Console.ReadLine();
			Console.Write("Children: (ex. 1): ");
			string childern = Console.ReadLine();
			Console.Write("Infants: (ex. 1): ");
			string infants = Console.ReadLine();

			string response = ApiRequest(from, to, departure, returnDate, currency, adults, childern, infants);
			try
			{
				var json = new WebClient().DownloadString(response);

				JObject results = JObject.Parse(json);

				foreach (var result in results["results"])
				{
					var price = result["fare"]["total_price"];
					
					foreach (var item in result["itineraries"])
					{
						var outbound = item["outbound"];

						foreach (var flight in outbound["flights"])
						{
							Console.WriteLine("Departure: {0} ({1})", 
							                  flight["origin"]["airport"], 
							                  readIataFromFile(flight["origin"]["airport"].ToString()));
							Console.WriteLine("Arriving: {0} ({1})", 
							                  flight["destination"]["airport"], 
							                  readIataFromFile(flight["destination"]["airport"].ToString()));
							Console.WriteLine("Deparutre date: {0} —— Arriving date: {1}", 
							                  flight["departs_at"], 
							                  flight["arrives_at"]);
							Console.WriteLine("Price == {0}", price + " " + currency);
							Console.WriteLine("———————————————————————————————————");
						}
					}
				}

				}
			catch (Exception ex)
			{
				Console.WriteLine("Ups, something went wrong :(");
				Console.WriteLine(ex.Message);
			}
		}



		private static string readIataFromFile(string iataCode)
		{
			string readFile = File.ReadAllText(@"iata.txt");
			JObject json = JObject.Parse(readFile);

			foreach (var item in json["response"])
			{
				//Console.WriteLine(item);
				if (item["iata"].ToString() == iataCode)
				{
					return item["name"].ToString();
				}
			}

			return "No such IATA code";

		}

		private static string iataCodeToName(string iataCode)
		{ 
			string iataApiRequet = @"https://iatacodes.org/api/v6/airports?api_key=c8f2e1db-bb9d-444c-827d-0ed4d18a3d68";
			var iataJson = new WebClient().DownloadString(iataApiRequet);
			JObject iataResult = JObject.Parse(iataJson);

			foreach (var item in iataResult["response"])
			{
				//Console.WriteLine(item);

				if (item["code"].ToString() == iataCode)
				{
					return item["name"].ToString();
				}

			}
			return "No such IATA code";

		}


		private static string ApiRequest(
			string origin, string destination, string departure, string back, string currency, string adults,
			string children, string infants
		) {
			return @"https://api.sandbox.amadeus.com/v1.2/flights/low-fare-search?" +
							"apikey=ZvwHAFaEHpBfXfHRox9XlDVHQOjX9I5A&" +
							"origin=" + origin +
							"&destination=" + destination + 
							"&departure_date=" + departure + 
							"&return_date=" + back + 
							"&currency=" + currency +
							"&adults=" + adults + 
							"&children" + children +
							"&infants" + infants +
							"&number_of_results=5";
		}



	}




}