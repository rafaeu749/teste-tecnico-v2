using NBomber.Contracts.Stats;
using NBomber.CSharp;
using System.Net.Http.Json;
using Thunders.TechTest.ApiService.Queues;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.Tests;

public class TollRegisterStressTest
{
    [Fact]
    public void Should_Stress_Post_TollRegister()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7405")
        };

        var plazaIdList = new[]{
            new Guid("eb5b2fcf-5d75-49a2-a631-1e2384377a76"),
            new Guid("97a8ed74-6c40-41fd-a8c3-0f9e23f1b809"),
            new Guid("ace3fa7f-313f-4c33-a69b-06d0b1c033a0")
        };

        var vehicleTypeList = Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>().ToList();

        var scenario = Scenario.Create("post_toll_register", async context =>
        {
            var registerData = new TollRegisterMessage(
                Id: Guid.NewGuid(),
                RegisteredAt: DateTime.UtcNow,
                AmountPaid: Random.Shared.Next(5, 50),
                VehicleType: vehicleTypeList[Random.Shared.Next(3)],
                TollPlazaId: plazaIdList[Random.Shared.Next(plazaIdList.Length)]
            );

            var response = await httpClient.PostAsJsonAsync("/api/tollregister", registerData);
            return response.IsSuccessStatusCode
                ? Response.Ok(statusCode: response.StatusCode.ToString())
                : Response.Fail(statusCode: response.StatusCode.ToString());
        });

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithTestSuite("TollStressSuite")
            .WithTestName("PostTollRegisterLoad")
            .WithReportFormats(ReportFormat.Html)
            .WithReportFileName("toll_post_stress")
            .WithReportFolder("./load-reports")
            .Run();
    }
}