using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace FirelyClient.Services
{
    public class FirelyServer : IFirelyServer
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string FirelyServerURL = string.Empty;
        
        private FhirClient myFhirClient;

        public FirelyServer(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            FirelyServerURL = config["FirelyServer"];
            myFhirClient = new FhirClient(FirelyServerURL);
        }       

        public async Task<IEnumerable<Patient>> GetPatients(string serverUrl = "")
        {
            List<Patient> result = new List<Patient>();
            if (!string.IsNullOrEmpty(serverUrl))            
                myFhirClient = new FhirClient(serverUrl);               
           
            var patientBundle = await myFhirClient.GetAsync("/Patient/");           
            foreach (var patient in ((Bundle)patientBundle).Entry)
            {
                result.Add((Patient)patient.Resource);
            }
            return result;
          
        }

        public async Task<Patient> GetPatient(string patientId, string serverUrl = "")
        {            
            if (!string.IsNullOrEmpty(serverUrl))
            {
                myFhirClient = new FhirClient(serverUrl);
                FirelyServerURL = serverUrl;
            }
            var location = new Uri(FirelyServerURL + "/Patient/" + patientId);          
            var patientBundle = await myFhirClient.ReadAsync<Patient>(location);
            
            return patientBundle;
        }
        
    }
}
