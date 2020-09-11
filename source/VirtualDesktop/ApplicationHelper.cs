using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WindowsDesktop.Interop;
using JetBrains.Annotations;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsDesktop
{
	public static class ApplicationHelper
	{
		internal static ApplicationView GetApplicationView(this IntPtr hWnd)
		{
			return ComInterface.ApplicationViewCollection.GetViewForHwnd(hWnd);
		}

		[CanBeNull]
		public static string GetAppId(IntPtr hWnd)
		{
			VirtualDesktopHelper.ThrowIfNotSupported();

			try
			{
				return hWnd.GetApplicationView().GetAppUserModelId();
			}
			catch (COMException ex) when (ex.Match(HResult.TYPE_E_ELEMENTNOTFOUND))
			{
				return null;
			}
		}

		public static Task StartSTATask(Action func)
		{
			var tcs = new TaskCompletionSource<object>();
			var thread = new Thread(() =>
			{
				try
				{
					func();
					tcs.SetResult(null);
				}
				catch (Exception e)
				{
					tcs.SetException(e);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			return tcs.Task;
		}
	}
}
