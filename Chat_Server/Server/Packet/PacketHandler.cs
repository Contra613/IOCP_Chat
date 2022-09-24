using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_ChatHandler(PacketSession session, IPacket packet)
	{
		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;

		// ClientSession이 없다면 그냥 Return
		if (clientSession.Room == null)
			return;

		GameRoom room = clientSession.Room;		// GameRoom을 ClientSession에 있는 Room에 연결
		room.Push(
			() => room.Broadcast(clientSession, chatPacket.chat)	// Room에서 실행할 작업을 추가
		);
	}
}
