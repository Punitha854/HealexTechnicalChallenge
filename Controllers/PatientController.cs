using FirelyClient.Helpers;
using FirelyClient.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace FirelyClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IFirelyServer _firelyServer;
        private RestAPI restAPI = new RestAPI();

        public PatientController(IFirelyServer firelyServer)
        {
            _firelyServer = firelyServer;
        }

        /// <summary>
        /// Get All Patients from the given FHIR server. By default Firely server is taken
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetPatients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(string serverUrl = "https://server.fire.ly")
        {
            try
            {
                IEnumerable<Patient> patients = await _firelyServer.GetPatients(serverUrl);
                return Ok(patients);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get a patient by its Id from the given FHIR server.By default firely server is taken
        /// </summary>
        /// <param name="PatientId">Id of the patient assigned from the respective server</param>
        /// <param name="serverUrl">The End point of the FHIR server</param>
        /// <returns></returns>
        [HttpGet("{PatientId}", Name = "GetPatientById")]
        public async Task<ActionResult<Patient>> GetPatientById(string PatientId, string serverUrl = "https://server.fire.ly")
        {
            try
            { 
                Patient patient = await _firelyServer.GetPatient(PatientId, serverUrl);
            return Ok(patient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
