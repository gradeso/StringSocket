using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
	public class BoggleService : IBoggleService
	{
		
		/// <summary>
		/// The most recent call to SetStatus determines the response code used when
		/// an http response is sent.
		/// </summary>
		/// <param name="status"></param>
		private static void SetStatus(HttpStatusCode status)
		{
			WebOperationContext.Current.OutgoingResponse.StatusCode = status;
		}

		/// <summary>
		/// Returns a Stream version of index.html.
		/// </summary>
		/// <returns></returns>
		public Stream API()
		{
			SetStatus(OK);
			WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
			return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
		}

		public string AttemptJoin()
		{
			throw new NotImplementedException();
		}

		public void CancelJoin()
		{
			throw new NotImplementedException();
		}

		public string playWordInGame(bool maybeYes)
		{
			throw new NotImplementedException();
		}

		public string PlayWordInGame(string GameID)
		{
			throw new NotImplementedException();
		}

		public void SaveUserID()
		{
			throw new NotImplementedException();
		}
	}
}
