using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	// GameRoom은 Client를 모아두고 관리하는 역활을 한다.
	// GameRoom은 Server가 관리한다.
	class GameRoom : IJobQueue
	{
		List<ClientSession> _sessions = new List<ClientSession>();
		JobQueue _jobQueue = new JobQueue();
		List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

		public void Push(Action job)
		{
			_jobQueue.Push(job);
		}

		// 모든 ClientSession을 돌면서 실행해야하는 작업들을 한꺼번에 보낸다?
		public void Flush()
		{
			// N ^ 2
			foreach (ClientSession s in _sessions)
				s.Send(_pendingList);

			//Console.WriteLine($"Flushed {_pendingList.Count} items");
			_pendingList.Clear();	// 다 보낸 작업들은 깨끗하게 지운다?
		}

		// GameRoom에 있는 모든 ClientSession에 Packet을 뿌려준다.
		public void Broadcast(ClientSession session, string chat)
		{
			S_Chat packet = new S_Chat();
			packet.playerId = session.SessionId;
			packet.chat =  $"ID({packet.playerId}) {chat}";
			ArraySegment<byte> segment = packet.Write();	// 보내야 하는 packet 작성

			_pendingList.Add(segment);			
		}

		// GameRoom에 입장
		public void Enter(ClientSession session)
		{
			_sessions.Add(session);
			session.Room = this;
		}

		// GameRoom에 퇴장
		public void Leave(ClientSession session)
		{
			_sessions.Remove(session);
		}
	}
}
