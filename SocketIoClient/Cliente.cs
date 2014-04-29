using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebSocket4Net;

namespace SocketIoClient
{
    public class Cliente
    {
        private SocketIOHandshake handshake;
        private HttpClient httpCliente;
        private WebSocket websocket;
        private string url;
        public Cliente(string url)
        {
            this.url = url;
            handshake = new SocketIOHandshake(null);
        }

        public async void Conectar()
        {
            Uri uri = new Uri(url);
            string key = await requestHandshake(url);
            handshake.UpdateFromSocketIOResponse(key);
            System.Diagnostics.Debug.WriteLine(handshake.SID);
            websocket = new WebSocket("ws://" + uri.Host + ":" + uri.Port + "/socket.io/1/websocket/" + handshake.SID);
            websocket.Opened += websocket_Opened;
            websocket.MessageReceived += websocket_MessageReceived;
            websocket.Closed += websocket_Closed;
            websocket.Open();
        }

        void websocket_Closed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Websocket cerrado");
        }

        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        void websocket_Opened(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("conectado");
        }

        public void Desconectar()
        {
            if (websocket == null||websocket.State == WebSocketState.Closed)
            {
                MessageBox.Show("Conexion no creada");
            }
            else
            {
                websocket.Close();
            }
        }

        public async Task<string> requestHandshake(string url)
        {
            httpCliente = new HttpClient();
            System.Diagnostics.Debug.WriteLine(handshake.SID);
            string key = await httpCliente.GetStringAsync(new Uri(string.Format("{0}/socket.io/1/websocket/?t=" + handshake.SID, url)));
            System.Diagnostics.Debug.WriteLine("request handshake: " + key);
            return key;
        }
    }
}
