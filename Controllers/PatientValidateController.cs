using FirelyClient.Helpers;
using FirelyClient.Services;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using Microsoft.AspNetCore.Mvc;

namespace FirelyClient.Controllers
{
    [ApiController]
    [Route("[controller]")]  
    public class PatientValidateController : Controller
    {
        private readonly IFirelyServer _firelyServer;
        private string CurDir = Environment.CurrentDirectory;
        private FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = true,  });
        private IResourceResolver resourceResolver;
        private ValidationSettings validationSettings;
        private Validator validator ;
        private RestAPI restAPI = new();
        private FhirJsonParser fhirParser = new FhirJsonParser();

        private const string profileUrl = "https://www.medizininformatik-initiative.de/fhir/core/modul-person/StructureDefinition/Patient";

        /// <summary>
        /// Patient validate
        /// </summary>
        /// <param name="firelyServer"></param>
        public PatientValidateController(IFirelyServer firelyServer)
        {
            _firelyServer = firelyServer;
            resourceResolver = new CachedResolver(new MultiResolver
                    (ZipSource.CreateValidationSource(), new DirectorySource($"{CurDir}\\Profiles", new DirectorySourceSettings() { IncludeSubDirectories = true, })));
            validationSettings = new ValidationSettings()
            {
                ResourceResolver = resourceResolver,
            };
            validator = new Hl7.Fhir.Validation.Validator(validationSettings);
        }


        /// <summary>
        /// Validates a given patient of the given fhir server by its ID. Note: the default fhir server https://server.fire.ly
        /// </summary>
        /// <param name="PatientId">Id of the patient assigned from the respective server</param>
        /// <param name="ServerName">The End point of the FHIR server</param>
        /// <returns></returns>
        [HttpGet("{PatientId}", Name = "ValidatePatientById")]
        public async Task<OperationOutcome> ValidateById(string PatientId, string ServerEndPoint = "https://server.fire.ly")
        {
            try
            {
                
                if (ServerEndPoint == "https://server.fire.ly")
                {
                    //Approach 1 direct through API 
                  string api = String.Format("https://server.fire.ly/Patient/{0}/$validate?profile={1}", PatientId,profileUrl);
                  string response = await restAPI.getAPIString(api);                                 
                  OperationOutcome outcome = fhirParser.Parse<OperationOutcome>(response);
                  return outcome;
                }
                else
                {
                    // approach for local server using profile directory
                    Patient patient = await _firelyServer.GetPatient(PatientId, ServerEndPoint);
                    var outcome = validator.Validate(patient);
                    return outcome;
                }
                //string patientJson = fhirJsonSerializer.SerializeToString(patient);
                //System.IO.File.WriteAllText("C:\\FHIR\\FirelyClient\\FirelyClient\\FirelyClient\\PatientSample.json", patientJson);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Validates all the patients of the given fhir server .Note: the default fhir server https://server.fire.ly
        /// </summary>
        /// <param name="ServerEndPoint">The FHIR server end point.</param>
        /// <returns></returns>
        [HttpGet(Name = "ValidatePatientsByServer")]
        public async Task<IEnumerable<OperationOutcome>> ValidatePatientsByServer(string ServerEndPoint = "https://server.fire.ly" )
        {
            List<OperationOutcome> outcomes = new List<OperationOutcome>();
            try
            {               
                var patients = await _firelyServer.GetPatients();                 
                foreach (Patient patient in patients)
                { 
                    if (ServerEndPoint == "https://server.fire.ly")
                    {
                        //Approach 1 direct through API 
                        string api = String.Format("https://server.fire.ly/Patient/{0}/$validate?profile={1}", patient.Id, profileUrl);                       
                        OperationOutcome outcome = fhirParser.Parse<OperationOutcome>( await restAPI.getAPIString(api));
                        outcomes.Add(outcome);
                    }
                    else
                    {
                        // approach for local server using profile directory                      
                        var outcome = validator.Validate(patient);
                        outcomes.Add(outcome);
                    }
                }               

            }
            catch (Exception ex)
            {   
                throw;
            }
            return outcomes;
        }

    }
}
