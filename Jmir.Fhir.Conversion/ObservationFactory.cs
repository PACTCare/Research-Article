using System;

namespace Jmir.Fhir.Conversion
{
  using System.Collections.Generic;

  using Common.Service.GattService;

  using Hl7.Fhir.Model;

  /// <summary>
  /// Sample class to show how a glucose measurement taken from a GATT compliant device can be converted into a FHIR resource
  /// </summary>
  public static class ObservationFactory
  {
    public static Observation FromMeasurement(GlucoseMeasurementValue measurement)
    {
      var observation = new Observation
                          {
                            Identifier =
                              new List<Identifier>
                                {
                                  new Identifier
                                    {
                                      System = "http://www.bmc.nl/zorgportal/identifiers/observations",
                                      Value = "6323",
                                      Use = Identifier.IdentifierUse.Official
                                    }
                                },
                            Status = ObservationStatus.Final,
                            Code = new CodeableConcept
                                     {
                                       Coding = new List<Coding>
                                                  {
                                                    new Coding
                                                      {
                                                        System = "http://loinc.org", Code = "15074-8", Display = "Glucose [Moles/volume] in Blood"
                                                      }
                                                  }
                                     },
                            Issued = DateTimeOffset.UtcNow,
                            Value = new SimpleQuantity
                                      {
                                        Value = (decimal)measurement.GlucoseConcentrationMolL,
                                        Unit = "mmol/l",
                                        System = "http://unitsofmeasure.org",
                                        Code = "mmol/l"
                                      }
                          };

      if (measurement.BaseTime != null)
      {
        observation.Effective = new Period { StartElement = new FhirDateTime(new DateTimeOffset(measurement.BaseTime.Value)) };
      }

      return observation;
    }
  }
}
