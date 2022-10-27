using System;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;


public class NaturalAccountRollUpTests : TestBase
{
    [Fact]
    public async Task NaturalAccountRollUpTest1()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.ErpAccountRollup.ExecuteAsync("410004");

        var data = result.ReadData();

        data.ShouldNotBeNull();

        data.ErpAccount.ShouldNotBeNull();
        data.ErpAccount.Code.ShouldBe("410004");
        data.ErpAccount.Parent.ShouldNotBeNull();
        data.ErpAccount.Parent.Code.ShouldBe("41000D");

        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "410004").ShouldBeFalse();

        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "41000D").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "41000C").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "41000B").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "41000A").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "4XXXXX").ShouldBeTrue();
        
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "41000Z").ShouldBeFalse();
    }

    [Fact]
    public async Task NaturalAccountRollUpTest2()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.ErpAccountRollup.ExecuteAsync("440011");

        var data = result.ReadData();

        data.ShouldNotBeNull();

        data.ErpAccount.ShouldNotBeNull();
        data.ErpAccount.Code.ShouldBe("440011");
        data.ErpAccount.Parent.ShouldNotBeNull();
        data.ErpAccount.Parent.Code.ShouldBe("44000D");

        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "440011").ShouldBeFalse();
                                                                   
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000D").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000C").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000B").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000A").ShouldBeTrue();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "4XXXXX").ShouldBeTrue();
                                                                   
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000Z").ShouldBeFalse();
    }

    [Fact]
    public async Task NaturalAccountRollUpTest3()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.ErpAccountRollup.ExecuteAsync("Z40011");

        var data = result.ReadData();

        data.ShouldNotBeNull();

        data.ErpAccount.ShouldBeNull();


        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "440011").ShouldBeFalse();

        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000D").ShouldBeFalse();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000C").ShouldBeFalse();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000B").ShouldBeFalse();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000A").ShouldBeFalse();
        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "4XXXXX").ShouldBeFalse();

        DoesNaturalAccountRollUp.NaturalAccount(data.ErpAccount, "44000Z").ShouldBeFalse();
    }
}

