using FirelyClient.Helpers;
using FirelyClient.Services;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirelyClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidateController : Controller
    {
        private readonly IFirelyServer _firelyServer;
        private RestAPI restAPI = new RestAPI();

        public ValidateController(FirelyServer firelyServer)
        {
            _firelyServer = firelyServer;
        }

        [HttpGet]
        public async Task<OperationOutcome> Validate(string id)
        {
            try
            {
                Patient patient = await _firelyServer.GetPatient(id);
                string api = "https://server.fire.ly/Patient/731b9ec4-57d6-4301-8716-b941c52ceb9b/$validate?profile=https://www.medizininformatik-initiative.de/fhir/core/modul-person/StructureDefinition/Patient";
                patient.Meta = new Meta()
                {
                    Profile = new List<string>()
                    {
                        "https://www.medizininformatik-initiative.de/fhir/core/modul-person/StructureDefinition/Patient"
                    }

                };
                patient.Identifier = new List<Identifier>()
                {
                    new Identifier("",""),
                };

               
                FhirJsonSerializer fhirJsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = true, });
                string patientJson = fhirJsonSerializer.SerializeToString(patient);

                IResourceResolver resourceResolver = new CachedResolver(new MultiResolver
                    (ZipSource.CreateValidationSource(), new DirectorySource("C:\\FHIR\\FirelyClient\\FirelyClient\\FirelyClient\\Profiles", new DirectorySourceSettings() { IncludeSubDirectories = true, })));

                System.IO.File.WriteAllText("C:\\FHIR\\FirelyClient\\FirelyClient\\FirelyClient\\PatientSample.json", patientJson);
                ValidationSettings validationSettings = new ValidationSettings()
                {
                    ResourceResolver = resourceResolver,
                };
                Hl7.Fhir.Validation.Validator validator = new Hl7.Fhir.Validation.Validator(validationSettings);

                var outcome = validator.Validate(patient);

                return outcome;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //[HttpGet(Name = "ValidatePatients")]
        //public async Task<OperationOutcome> ValidatePatients(string ServerName)
        //{
        //    try
        //    {
        //        var patients = await _firelyServer.GetPatients();

        //        //Hl7.Fhir.Validation.Validator validator = new Hl7.Fhir.Validation.Validator();
                
        //        //var outcome = validator.Validate(patient);

        //        return null;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
    }
}
