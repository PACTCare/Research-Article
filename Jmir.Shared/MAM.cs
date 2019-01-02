namespace Jmir.Shared
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading.Tasks;

  using Tangle.Net.Cryptography;
  using Tangle.Net.Entity;
  using Tangle.Net.Mam.Entity;
  using Tangle.Net.Mam.Merkle;
  using Tangle.Net.Mam.Services;
  using Tangle.Net.ProofOfWork.Service;
  using Tangle.Net.Repository;
  using Tangle.Net.Repository.Client;

  public class MAM
  {
    private readonly MamChannel channel;

    public MAM(ILogger logger)
    {
      this.Logger = logger;

      var repository = new RestIotaRepository(
        new FallbackIotaClient(
          new List<string>
            {
              "https://invalid.node.com:443",
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

      this.ChannelFactory = new MamChannelFactory(CurlMamFactory.Default, CurlMerkleTreeFactory.Default, repository);

      var seed = Seed.Random();
      var channelKey = Seed.Random();

      this.Logger.Log($"Seed: {seed.Value}");
      this.Logger.Log($"ChannelKey: {channelKey.Value}");

      this.channel = this.ChannelFactory.Create(Mode.Restricted, seed, SecurityLevel.Medium, channelKey);
    }

    public ILogger Logger { get; }

    private MamChannelFactory ChannelFactory { get; }

    public async Task<TimeObj> PublishMessage(string text)
    {
      // Create Message
      var timeObj = new TimeObj();
      var timer = Stopwatch.StartNew();
      var message = this.channel.CreateMessage(TryteString.FromUtf8String(text));
      timer.Stop();
      var timespan = timer.Elapsed;
      timeObj.CreateTime = $"{timespan.Seconds:00}{timespan.Milliseconds:00}";

      // Publish Message
      timer = Stopwatch.StartNew();
      await this.channel.PublishAsync(message);
      timer.Stop();
      timespan = timer.Elapsed;
      timeObj.AttachTime = $"{timespan.Seconds:00}{timespan.Milliseconds:00}";
      return timeObj;
    }
  }
}