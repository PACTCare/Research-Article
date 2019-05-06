namespace Pact.Fhir.Iota.Repository
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading.Tasks;

  using Hl7.Fhir.Model;

  using Pact.Fhir.Core.Repository;
  using Pact.Fhir.Core.Usecase.CreateResource;
  using Pact.Fhir.Iota.Serializer;
  using Pact.Fhir.Iota.Services;

  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.Repository;

  using ResourceEntry = Pact.Fhir.Iota.Entity.ResourceEntry;

  /// <summary>
  /// Inject repository for now. Core Factory needs to be adjusted or injection has to be done another way, later
  /// </summary>
  public class IotaFhirRepository : FhirRepository
  {
    public IotaFhirRepository(IIotaRepository repository, IFhirTryteSerializer serializer, IResourceTracker resourceTracker)
    {
      this.Serializer = serializer;
      this.ResourceTracker = resourceTracker;
      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);
      this.SubscriptionFactory = new MamChannelSubscriptionFactory(repository, CurlMamParser.Default, CurlMask.Default);
      this.ResultTimes = new List<TimeTrackingEntry>();
      this.ReadTimes = new List<ReadTrackingEntry>();
    }

    // Working with low security level for the sake of speed
    private static int SecurityLevel => Tangle.Net.Cryptography.SecurityLevel.Medium;

    private MamChannelFactory ChannelFactory { get; }

    public IResourceTracker ResourceTracker { get; set; }

    private IFhirTryteSerializer Serializer { get; }

    private MamChannelSubscriptionFactory SubscriptionFactory { get; }

    public List<TimeTrackingEntry> ResultTimes { get; set; }
    public List<ReadTrackingEntry> ReadTimes { get; set; }

    /// <inheritdoc />
    public override async Task<DomainResource> CreateResourceAsync(DomainResource resource)
    {
      // Setup for unlinked resources (not linked to a user seed)
      // User seed handling has to be implemented later (must conform FHIR specifications)
      var seed = Seed.Random();
      var channelKey = Seed.Random();

      // New FHIR resources SHALL be assigned a logical and a version id. Take hash of first message for that
      var rootHash = CurlMerkleTreeFactory.Default.Create(seed, 0, 1, SecurityLevel).Root.Hash;
      this.PopulateMetadata(resource, rootHash.Value, rootHash.Value);

      var channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel, channelKey);

      var timer = Stopwatch.StartNew();
      var message = channel.CreateMessage(this.Serializer.Serialize(resource));
      timer.Stop();
      var createTime = timer.ElapsedMilliseconds;

      // Publish Message
      timer = Stopwatch.StartNew();
      await channel.PublishAsync(message);
      timer.Stop();
      var attachTime = timer.ElapsedMilliseconds;

      this.ResultTimes.Add(new TimeTrackingEntry { AttachTime = attachTime, CreateTime = createTime });

      // After successfully publishing a message, we can save that to the ResourceTracker.
      // This will allow us to retrieve the channelKey for other usecases
      this.ResourceTracker.AddEntry(new ResourceEntry { ChannelKey = channelKey, MerkleRoots = new List<Hash> { rootHash } });

      return resource;
    }

    /// <inheritdoc />
    public override async Task<DomainResource> ReadResourceAsync(string id)
    {
      UnmaskedAuthenticatedMessage message = null;
      // Get the tracked resource associated with the given id and filter the MAM root from that
      foreach (var entry in this.ResourceTracker.Entries)
      {
        var timer = Stopwatch.StartNew();
        timer.Start();
        var resourceRoot = entry.MerkleRoots.First();

        // now we can read the FHIR resource from the MAM stream
        var subscription = this.SubscriptionFactory.Create(
          resourceRoot,
          Mode.Restricted,
          entry.ChannelKey);

        message = await subscription.FetchSingle(resourceRoot);
        timer.Stop();
        this.ReadTimes.Add(new ReadTrackingEntry { ReadTime = timer.ElapsedMilliseconds });
      }

      var resource = this.Serializer.Deserialize<DomainResource>(message.Message);
      return resource;
    }
  }
}