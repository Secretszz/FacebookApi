// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		ILoginListener.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:36:11
// *******************************************

namespace Bridge.FacebookApi
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILoginListener
	{
		void OnSuccess();

		void OnCancel();

		void OnError(int errCode, string errMsg);
	}
}