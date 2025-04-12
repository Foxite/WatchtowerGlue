namespace AuthenticatedGlue.Services;

public interface IReplayCache {
	/// <summary>
	/// Returns true if the authorization is safe, and false if it is potentially a replay.
	/// Also saves the authorization in cache. Thus, calling this twice returns true, then false.
	/// </summary>
	bool Check(string authorization);
}

public class NoopReplayCache : IReplayCache {
	public bool Check(string authorization) => true;
}
