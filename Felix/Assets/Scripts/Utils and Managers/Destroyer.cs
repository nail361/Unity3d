using UnityEngine;

public class Destroyer : MonoBehaviour {

    [SerializeField]
    private int DestroyAfterMilliseconds = 0;

	void Start () {
        Timers.AddTimer("destroyer", 0, DestroyAfterMilliseconds, 1000, null, OnTimerComplete);
	}

    public void DestroyNow()
    {
        Timers.RemoveTimer("destroyer");
        Destroy(gameObject);
    }

	void OnTimerComplete () {
        Destroy(gameObject);
	}
}
