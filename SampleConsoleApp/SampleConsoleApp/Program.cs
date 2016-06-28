using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Configuration;
using RestSharp;

namespace SampleConsoleApp
{
    class Program
    {
        static string apiKey = ConfigurationManager.AppSettings["apiKey"];
        static string apiURL = ConfigurationManager.AppSettings["apiUrl"];

        static void Main(string[] args)
        {
            //Creating the client to use for all the requests
            //This takes the base api URL
            var restClient = new RestClient(apiURL);

            //Passing in the client to get the appointments
            var appointments = FindAppointments(restClient);
            Console.WriteLine(String.Format("{0} - Results Found", appointments.Total));

            //Lets loop through the search bundle and check each entry for a patient participant (they should all have one)
            foreach(var appointment in appointments.Entry)
            {
                Appointment appt = (Appointment)appointment.Resource;
                Console.WriteLine(String.Format("{0} - {1}", appt.Start, appt.Description));
                
                //Finding the patient participant
                var patientParticipant = appt.Participant.FirstOrDefault(part => part.Actor.Reference.StartsWith("Patient"));
                if(patientParticipant != null)
                {
                    //We can now use the patient participant reference as the actual resource url ex: Patient/++123456
                    var patientRequest = new RestRequest(patientParticipant.Actor.Reference, Method.GET);
                    patientRequest.AddQueryParameter("apiKey", apiKey); //We need to pass in the app key

                    //Lets do the request and make sure the calll was successful
                    var response = restClient.Get(patientRequest);                    
                    if(response.ResponseStatus == ResponseStatus.Completed)
                    {
                        //We need to use the FHIRParser class to properly deserialize the JSON from the request
                        var patient = (Patient)FhirParser.ParseFromJson(response.Content);
                        Console.WriteLine(patient.BirthDate);
                    }                    
                }
            }
            Console.ReadKey();
        }

        //Returns a search bundle
        static Bundle FindAppointments(RestClient client)
        {
            var appointmentRequest = new RestRequest("Appointment", Method.GET);
            appointmentRequest.AddQueryParameter("apiKey", apiKey); //We need to pass in the app key
            appointmentRequest.AddQueryParameter("date.after", "6/10/2015");
            appointmentRequest.AddQueryParameter("date.before", "6/30/2015");
            appointmentRequest.AddQueryParameter("practitioner.identifier", "1071368");

            //Lets do the request with our constructed request
            var response = client.Get(appointmentRequest);
            if (response.ResponseStatus == ResponseStatus.Completed)
            {
                //We need to use the FHIRParser class to properly deserialize the JSON from the request
                var appointments = (Bundle)FhirParser.ParseFromJson(response.Content);

                return appointments;
            }

            return null;

        }
    }
}
