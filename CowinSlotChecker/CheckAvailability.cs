using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CowinPro.DataTypes;
using Newtonsoft.Json;

namespace CowinPro
{
    public class CheckAvailability
    {
        public  CheckAvailability(string pincode)
        {
            _pincode = pincode;
        }
        public string _pincode;
        public string url = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByPin";
        public string date = DateTime.Today.ToString("dd-MM-yyyy");
        public string result;                 
        public List<ReturnData>Check(CancellationToken cancellationToken)
        {
            var parameters = Build(_pincode, date);
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "hi_IN");     
            HttpResponseMessage response = null;
            Task<HttpResponseMessage> task = null;
            task = Task.Run<HttpResponseMessage>(async () => await httpClient.GetAsync(url + parameters));
            List<ReturnData> returnDatas = new List<ReturnData>();       
            while (returnDatas.Count == 0)
            {
                if (task != null)
                {
                    response = task.GetAwaiter().GetResult();
                    var task2 = Task.Run<string>(async () => await response.Content.ReadAsStringAsync());
                    result = task2.GetAwaiter().GetResult();
                    Root json = JsonConvert.DeserializeObject<Root>(result);
                    foreach (var certer in json.centers)
                    {
                        foreach (var session in certer.sessions)
                        {
                            if (session.available_capacity_dose1 > 0 && session.min_age_limit == 18)
                            {
                                ReturnData returnData = new ReturnData();
                                returnData.date = session.date;
                                returnData.available = session.available_capacity_dose1;
                                returnData.Vaccinename = session.vaccine;
                                returnData.Address = certer.address;
                                returnDatas.Add(returnData);

                            }
                        }
                    }
                }
            }
            return returnDatas;
        }
        private string Build(string pincode,string date)
        {
            return "?pincode=" + pincode + "&date=" + date;
        }

    }
}
