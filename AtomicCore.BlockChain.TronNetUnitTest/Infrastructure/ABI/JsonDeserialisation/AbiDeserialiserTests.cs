using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AtomicCore.BlockChain.TronNet.Tests.Infrastructure.ABI.JsonDeserialisation;

[TestClass]
public class AbiDeserialiserTests
{
  [TestMethod]
  public void CanDeserialiseJsonContract()
  {
    var contract = new SmartContract();
    contract.Name = "TestContract";
    contract.Abi = new SmartContract.Types.ABI();
    contract.Abi.Entrys.Add(new SmartContract.Types.ABI.Types.Entry()
    {
      Anonymous = false,
      Constant = false,
      Name = "TestFunction",
      Payable = false,
      Type = SmartContract.Types.ABI.Types.Entry.Types.EntryType.Function,
      Inputs = { new SmartContract.Types.ABI.Types.Entry.Types.Param() { Name = "testInput", Type = "string" } },
      Outputs = { new SmartContract.Types.ABI.Types.Entry.Types.Param() { Name = "testOutput", Type = "string" } },
    });

    var entries = contract.Abi.Entrys;
    var json = JsonSerializer.Serialize(entries, SerializerOptions);

    var testee = new ABIDeserialiser();
    var abi = testee.DeserialiseContract(json);

    Assert.AreEqual(1, abi.Functions.Length);
    Assert.AreEqual("testInput", abi.Functions[0].InputParameters[0].Name);
    Assert.AreEqual("string", abi.Functions[0].InputParameters[0].Type);
    Assert.AreEqual("testOutput", abi.Functions[0].OutputParameters[0].Name);
    Assert.AreEqual("string", abi.Functions[0].OutputParameters[0].Type);
  }

  private JsonSerializerOptions SerializerOptions => new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
  };
}