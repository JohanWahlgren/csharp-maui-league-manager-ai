using Android.App;
using Android.Runtime;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
