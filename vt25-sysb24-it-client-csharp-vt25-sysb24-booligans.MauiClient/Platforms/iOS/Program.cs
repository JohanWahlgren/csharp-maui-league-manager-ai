using ObjCRuntime;
using UIKit;

namespace vt25_sysb24_it_client_csharp_vt25_sysb24_booligans.MauiClient;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
