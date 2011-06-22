using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace iTunesRemote.Agent
{
	[RunInstaller(true)]
	public partial class AgentInstaller : Installer
	{
		public AgentInstaller()
		{
			InitializeComponent();
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			string url = Context.Parameters["url"];

			ExecuteCommandSync(
				String.Format("netsh http delete urlacl url=\"{0}\"", url));
		}

		public override void Install(IDictionary savedState)
		{
			System.Diagnostics.Debugger.Break();

			base.Install(savedState);

			string url = Context.Parameters["url"];
			string user = Context.Parameters["user"];
			string machine = Context.Parameters["machine"];

			ExecuteCommandSync(
				String.Format("netsh http add urlacl url=\"{0}\" user={1}\\{2}", url, machine, user));
		}

		public void ExecuteCommandSync(object command)
		{
			try
			{
				// create the ProcessStartInfo using "cmd" as the program to be run,
				// and "/c " as the parameters.
				// Incidentally, /c tells cmd that we want it to execute the command that follows,
				// and then exit.
				var procStartInfo =
					new ProcessStartInfo("cmd", "/c " + command);

				// The following commands are needed to redirect the standard output.
				// This means that it will be redirected to the Process.StandardOutput StreamReader.
				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.UseShellExecute = false;
				// Do not create the black window.
				procStartInfo.CreateNoWindow = true;
				// Now we create a process, assign its ProcessStartInfo and start it
				var proc = new Process();
				proc.StartInfo = procStartInfo;
				proc.Start();
				// Get the output into a string
				string result = proc.StandardOutput.ReadToEnd();
				// Display the command output.
				Console.WriteLine(result);
			}
			catch (Exception objException)
			{
				// Log the exception
			}
		}
	}
}