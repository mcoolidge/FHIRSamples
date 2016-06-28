﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System.Configuration;

namespace SampleConsoleApp
{
    class Program
    {
        static string apiKey = ConfigurationManager.AppSettings["apiKey"];
        static string apiURL = ConfigurationManager.AppSettings["apiUrl"];

        static void Main(string[] args)
        {

            var client = new FhirClient(new Uri(apiURL));

            var appointments = FindAppointments(client);

            Console.WriteLine(String.Format("{0} - Results Found", appointments.Total));

            foreach(var appointment in appointments.Entry)
            {
                Appointment appt = (Appointment)appointment.Resource;
                Console.WriteLine(String.Format("{0} - {1}", appt.Start, appt.Description));

            }
            Console.ReadKey();
        }

        static Bundle FindAppointments(FhirClient client)
        {

            var searchParams = new SearchParams();
            searchParams.Add("apiKey", apiKey);
            searchParams.Add("date.after", "6/10/2015");
            searchParams.Add("date.before", "6/30/2015");
            searchParams.Add("practitioner.identifier", "1071368");
            
            var appointments = client.Search<Appointment>(searchParams);

            return appointments;

        }
    }
}
