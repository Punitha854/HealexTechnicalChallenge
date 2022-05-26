using Hl7.Fhir.Model;

namespace FirelyClient.Services
{
    public interface IFirelyServer
    {
        Task<IEnumerable<Patient>> GetPatients(string serverUrl = "") ;
        Task<Patient> GetPatient(string id, string serverUrl = "");        
    }
}
