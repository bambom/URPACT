using UnityEngine;
using System.Collections;
using WebSocket4Net;

namespace WebSocketHandShake
{
    public class Handshake : MonoBehaviour
    {
        static Handshake msInstance = null;
        public static Handshake Instance { get { return msInstance ?? (msInstance = new Handshake()); } }
        public void HandCommand(WebSocket websocket)
        {
            //MainScript.Execute(new ShakeHandsCmd(),
            //    delegate(string err, Response response)
            //    {
            //        Debug.Log("loin");
            //        Global.ShowLoadingEnd();
            //        if (!string.IsNullOrEmpty(err))
            //        {
            //            MessageBoxWnd.Instance.Show(err, MessageBoxWnd.Style.OK);
            //            return;
            //        }

            //        websocket.OnHandshaked();
            //    });
        }
    }
}
