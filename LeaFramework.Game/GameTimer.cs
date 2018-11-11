// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Diagnostics;

namespace LeaFramework.Game
{
	public class GameTimer
	{
		private readonly double _secondsPerCount;
		private double _deltaTime;

		private long baseTime;
		private long pausedTime;
		private long stopTime;
		private long prevTime;
		private long currTime;

		private bool stopped;

		public GameTimer()
		{
			
			_deltaTime = -1.0;
			baseTime = 0;
			pausedTime = 0;
			prevTime = 0;
			currTime = 0;
			stopped = false;

			var countsPerSec = Stopwatch.Frequency;
			_secondsPerCount = 1.0 / countsPerSec;

		}

		public float TotalTime
		{
			get
			{
				if (stopped)
					return (float)((stopTime - pausedTime - baseTime) * _secondsPerCount);
				else
					return (float)((currTime - pausedTime - baseTime) * _secondsPerCount);
			}
		}
		public float DeltaTime => (float)_deltaTime;

		public float FrameTime { get; set; }

		public void Reset()
		{
			var curTime = Stopwatch.GetTimestamp();
			baseTime = curTime;
			prevTime = curTime;
			stopTime = 0;
			stopped = false;
		}

		public void Start()
		{
			var startTime = Stopwatch.GetTimestamp();
			if (stopped)
			{
				pausedTime += startTime - stopTime;
				prevTime = startTime;
				stopTime = 0;
				stopped = false;
			}
		}

		public void Stop()
		{
			if (!stopped)
			{
				var curTime = Stopwatch.GetTimestamp();
				stopTime = curTime;
				stopped = true;
			}
		}

		public void Tick()
		{
			if (stopped)
			{
				_deltaTime = 0.0;
				return;
			}
			//while (_deltaTime < FrameTime) {
			var curTime = Stopwatch.GetTimestamp();
			currTime = curTime;

			_deltaTime = (currTime - prevTime) * _secondsPerCount;
			//Thread.Sleep(0);
			//}
			prevTime = currTime;
			if (_deltaTime < 0.0)
				_deltaTime = 0.0;
		}
	}
}
