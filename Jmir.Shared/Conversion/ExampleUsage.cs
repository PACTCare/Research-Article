namespace Jmir.Shared.Conversion
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Common.Service.GattService;

  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;

  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class ExampleUsage
  {
    private static IIotaRepository Repository =>
      new RestIotaRepository(
        new FallbackIotaClient(
          new List<string>
            {
              "https://peanut.iotasalad.org:14265",
              "http://node04.iotatoken.nl:14265",
              "http://node05.iotatoken.nl:16265",
              "https://nodes.thetangle.org:443",
              "http://iota1.heidger.eu:14265",
              "https://nodes.iota.cafe:443",
              "https://potato.iotasalad.org:14265",
              "https://durian.iotasalad.org:14265",
              "https://turnip.iotasalad.org:14265",
              "https://nodes.iota.fm:443",
              "https://tuna.iotasalad.org:14265",
              "https://iotanode2.jlld.at:443",
              "https://node.iota.moe:443",
              "https://wallet1.iota.town:443",
              "https://wallet2.iota.town:443",
              "http://node03.iotatoken.nl:15265",
              "https://node.iota-tangle.io:14265",
              "https://pow4.iota.community:443",
              "https://dyn.tangle-nodes.com:443",
              "https://pow5.iota.community:443",
            },
          5000),
        new PoWSrvService());

    public async Task CreateAndUploadMeasurementAsFhir()
    {
      // For the sake of simplicity the measurement is created directly rather than read via bluetooth
      var measurement = new GlucoseMeasurementValue { GlucoseConcentrationMolL = 5.4f, BaseTime = DateTime.UtcNow };

      // The measurement can then be converted to a FHIR resource by following a simple factory pattern, that fills a blueprint with values
      var observation = ObservationFactory.FromMeasurement(measurement);

      // This FHIR resource now can be uploaded to the tangle
      var interactor = new CreateResourceInteractor(new IotaFhirRepository(Repository, new FhirJsonTryteSerializer(), new InMemoryResourceTracker()));

      // The result includes all necessary data to read the FHIR resource from e.g. an FHIR API
      var result = await interactor.ExecuteAsync(new CreateResourceRequest { Resource = observation });
    }
  }
}