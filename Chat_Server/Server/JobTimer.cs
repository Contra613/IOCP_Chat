using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	struct JobTimerElem : IComparable<JobTimerElem>
	{
		public int execTick;		// 실행 시간
		public Action action;		// 작업

		public int CompareTo(JobTimerElem other)
		{
			return other.execTick - execTick;
		}
	}

	class JobTimer
	{
		PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();	// 우선순위 큐
		object _lock = new object();

		public static JobTimer Instance { get; } = new JobTimer();

		// PriorityQueue<JobTimerElem>에 작업 실행 시간과 실행할 작업 추가 
		public void Push(Action action, int tickAfter = 0)
		{
			JobTimerElem job;
			job.execTick = System.Environment.TickCount + tickAfter;
			job.action = action;

			lock (_lock)
			{
				_pq.Push(job);
			}
		}

		// 
		public void Flush()
		{
			while (true)
			{
				int now = System.Environment.TickCount;		// 현재 시간

				JobTimerElem job;

				lock (_lock)
				{
					if (_pq.Count == 0)
						break;

					job = _pq.Peek();	// 훔쳐보기
					if (job.execTick > now)
						break;

					_pq.Pop();
				}

				job.action.Invoke();	// 작업 실행
			}
		}
	}
}
