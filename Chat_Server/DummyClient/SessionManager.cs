using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		List<ServerSession> _sessions = new List<ServerSession>();
		object _lock = new object();

		// 모든 ServerSession에 Chat packet을 보내기
		public void SendForEach()
		{
			lock (_lock)
			{
				foreach (ServerSession session in _sessions)
				{
					C_Chat chatPacket = new C_Chat();
					string userchat = Console.ReadLine();
					chatPacket.chat = $"{userchat}";
					ArraySegment<byte> segment = chatPacket.Write();

					session.Send(segment);


					Console.WriteLine("\n");
				}
			}
		}

		// ServerSession 생성
		public ServerSession Generate()
		{
			lock (_lock)
			{
				ServerSession session = new ServerSession();
				_sessions.Add(session);
				return session;
			}
		}
	}
}
