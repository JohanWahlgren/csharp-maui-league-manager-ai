using System.Threading.Tasks;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services
{
    public interface IOpenAIService
    {
        Task<string> GenerateTeamReportAsync(string teamInfo, string playerInfo);
    }
}