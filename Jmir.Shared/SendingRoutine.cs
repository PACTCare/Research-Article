namespace Jmir.Shared
{
  using System;
  using System.Collections.Generic;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Iota.Repository;
  using Pact.Fhir.Iota.Serializer;

  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  using Task = System.Threading.Tasks.Task;

  public class SendingRoutine
  {
    public SendingRoutine(ILogger logger, IProgressTracker tracker)
    {
      this.Logger = logger;
      this.Tracker = tracker;
    }

    public static Patient Patient =>
      new Patient
        {
          Name = new List<HumanName>
                   {
                     new HumanName { Use = HumanName.NameUse.Official, Prefix = new[] { "Mr" }, Given = new[] { "Max" }, Family = "Mustermann" }
                   },
          Identifier = new List<Identifier>
                         {
                           new Identifier { System = "http://ns.electronichealth.net.au/id/hi/ihi/1.0", Value = "8003608166690503" }
                         }
        };

    public ILogger Logger { get; }

    public IProgressTracker Tracker { get; }

    public async Task RunAsync(int rounds)
    {
      var repository = new RestIotaRepository(
        new FallbackIotaClient(
          new List<string>
            {
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

      var fhirRepository = new IotaFhirRepository(repository, new FhirJsonTryteSerializer(), new InMemoryResourceTracker());
      var resourceInteractor = new CreateResourceInteractor(fhirRepository);

      var n = 0;
      while (n < rounds)
      {
        n++;
        this.Tracker.Update(n);
        await resourceInteractor.ExecuteAsync(new CreateResourceRequest { Resource = Patient });
      }

      foreach (var trackingEntry in fhirRepository.ResultTimes)
      {
        this.Logger.Log($"Create: {trackingEntry.CreateTime:0000} | Attach: {trackingEntry.AttachTime:0000}");
      }
    }
  }
}