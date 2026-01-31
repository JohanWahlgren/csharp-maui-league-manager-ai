using vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient.Services;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetPlayersAsync();
    Task<Player> GetPlayerAsync(string playerNo);
}