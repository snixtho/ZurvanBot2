using System.Net;
using System.Net.Sockets;

namespace ZurvanBot.Util.Net {
    public class UdpClientEx : UdpClient {
        /// <summary>
        /// Whether the connection is active or not.
        /// </summary>
        public new bool Active => base.Active;
        
        /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class.</summary>
    /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
    public UdpClientEx() : base() {}

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class.</summary>
    /// <param name="family">One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values that specifies the addressing scheme of the socket. </param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="family" /> is not <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />. </exception>
    /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
    public UdpClientEx(AddressFamily family) : base(family) {}

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class and binds it to the local port number provided.</summary>
    /// <param name="port">The local port number from which you intend to communicate. </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> parameter is greater than <see cref="F:System.Net.IPEndPoint.MaxPort" /> or less than <see cref="F:System.Net.IPEndPoint.MinPort" />. </exception>
    /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
    public UdpClientEx(int port) : base(port) {}

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class and binds it to the local port number provided.</summary>
    /// <param name="port">The port on which to listen for incoming connection attempts. </param>
    /// <param name="family">One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values that specifies the addressing scheme of the socket. </param>
    /// <exception cref="T:System.ArgumentException">
    /// <paramref name="family" /> is not <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="port" /> is greater than <see cref="F:System.Net.IPEndPoint.MaxPort" /> or less than <see cref="F:System.Net.IPEndPoint.MinPort" />. </exception>
    /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
    public UdpClientEx(int port, AddressFamily family) : base(port, family) {}

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class and binds it to the specified local endpoint.</summary>
    /// <param name="localEP">An <see cref="T:System.Net.IPEndPoint" /> that respresents the local endpoint to which you bind the UDP connection. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="localEP" /> is null. </exception>
    /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
    public UdpClientEx(IPEndPoint localEP) : base(localEP) {}

      /// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.UdpClient" /> class and establishes a default remote host.</summary>
      /// <param name="hostname">The name of the remote DNS host to which you intend to connect. </param>
      /// <param name="port">The remote port number to which you intend to connect. </param>
      /// <exception cref="T:System.ArgumentNullException">
      /// <paramref name="hostname" /> is null. </exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException">
      /// <paramref name="port" /> is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />. </exception>
      /// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket. See the Remarks section for more information. </exception>
      public UdpClientEx(string hostname, int port) : base(hostname, port) {}
    }
}