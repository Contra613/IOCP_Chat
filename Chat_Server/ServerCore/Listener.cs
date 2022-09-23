using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	// Listener는 Server에서 실행
	// Client의 입장을 도와준다.
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;

		// 문지기 설정
		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			// Socket 생성
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;

			// 문지기 교육
			_listenSocket.Bind(endPoint);

			// 영업 시작
			// backlog : 최대 대기수
			_listenSocket.Listen(backlog);

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			// Server 입장(식당 입장)
			bool pending = _listenSocket.AcceptAsync(args);
			if (pending == false)
				OnAcceptCompleted(null, args);
		}

		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				// Client가 Server에 입장한 후 SessionManager에서 ClientSession을 생성해준다.
				Session session = _sessionFactory.Invoke();
				// Client가 Serveer에 입장했으니 Session에서 Packet을 받고, 보낼 준비를 한다.
				session.Start(args.AcceptSocket);
				// Client가 Serveer에 입장했으니 ClientSession에서 Connect 후에 할 작업을 실행
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}
			else
				Console.WriteLine(args.SocketError.ToString());

			RegisterAccept(args);
		}
	}
}
